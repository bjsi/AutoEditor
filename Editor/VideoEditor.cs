using AutoEditor.Helpers;
using AutoEditor.Models;
using CliWrap.Buffered;
using CliWrap.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AutoEditor.Editor
{
    public class VideoEditor
    {

        private readonly FileInfo _ffprobePath;
        private readonly FileInfo _ffmpegPath;
        private readonly FileInfo videoFile;

        public VideoEditor(FileInfo ffmpeg, FileInfo ffprobe, FileInfo videoFile)
        {
            ThrowIfInvalid(ffmpeg, ffprobe, videoFile);
            this._ffmpegPath = ffmpeg;
            this.videoFile = videoFile;
            this._ffprobePath = ffprobe;
        }

        private static void ThrowIfInvalid(FileInfo ffmpeg, FileInfo ffprobe, FileInfo videoFile)
        {
            ffmpeg.ThrowIfMissing("Path to FFMPEG does not exist");
            videoFile.ThrowIfMissing("Path to input file does not exist");
            ffprobe.ThrowIfMissing("Path to ffprobe does not exist");
        }

        private async Task<BufferedCommandResult> ExecuteFfprobeCommandAsync(IEnumerable<string> args)
        {
            return await CliWrap.Cli
                .Wrap(_ffprobePath.FullName)
                .WithArguments(args)
                .ExecuteBufferedAsync();
        }

        private async Task<BufferedCommandResult> ExecuteFfmpegCommandAsync(IEnumerable<string> args)
        {
            return await CliWrap.Cli
                .Wrap(_ffmpegPath.FullName)
                .WithArguments(args, false)
                .ExecuteBufferedAsync();
        }

        private async Task<int> GetAudioSamplingRate()
        {
            try
            {
                var args = new string[]
                {
                    "-show_streams",
                    "-of", "json",
                    videoFile.FullName
                };

                var output = await ExecuteFfprobeCommandAsync(args);
                var info = ParseFfmpegSamplingRate(output.StandardOutput);
                return info?.First().sample_rate ?? -1;

            }
            catch (CommandExecutionException ex)
            {
                Console.WriteLine("Failed to get audio sampling rate info because the call to ffmpeg failed.");
                return -1;
            }
        }

        private List<FFprobeStream> ParseFfmpegSamplingRate(string input)
        {
            return input?.Deserialize<RootFfprobeObject>().Streams;
        }

        /// <summary>
        /// https://superuser.com/questions/1183663/determining-audio-level-peaks-with-ffmpeg
        /// </summary>
        /// <returns></returns>
        public async Task<List<LoudnessInfo>> GetAudioLoudnessInfo()
        {
            var tmp = DateTime.Now.Ticks + "ffmpeg.tmp";

            try
            {
                // Samples the loudness roughly every 1 second
                var reset = await GetAudioSamplingRate() / 1000;

                var args = new string[]
                {
                    "-i", $"{videoFile.FullName}",
                    "-af", $"astats=metadata=1:reset={reset},ametadata=print:key=lavfi.astats.Overall.RMS_level:file={tmp}",
                    "-f", "null",
                    "-"
                };

                var info = await ExecuteFfmpegCommandAsync(args);
                var lines = await File.ReadAllLinesAsync(tmp);
                return ParseFfmpegLoudnessInfo(lines);

            }
            catch (CommandExecutionException e)
            {
                Console.WriteLine("Failed to get audio loudness info because the call to ffmpeg failed.");
                return null;
            }
            catch (IOException e)
            {
                Console.WriteLine("Failed to get audio loudness info because the call to ffmpeg failed.");
                return null;
            }
            finally
            {
                File.Delete(tmp);
            }
        }

        /// <summary>
        /// Parse the standard output of ffmpeg call.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private List<LoudnessInfo> ParseFfmpegLoudnessInfo(string[] input)
        {
            var ret = new List<LoudnessInfo>();
            if (input.Length < 2 || input.Length % 2 != 0) 
            {
                throw new InvalidDataException("Unexpected input to ParseFfmpegLoudnessInfo");
            }

            for (int i = 1; i < input.Length; i += 2)
            {
                var firstLine = input[i - 1];
                var secondLine = input[i];
                ret.Add(DeserializeLoudnessInfo(firstLine, secondLine));
            }
            return ret;
        }

        private LoudnessInfo DeserializeLoudnessInfo(string firstLine, string secondLine)
        {
            var info = new LoudnessInfo();

            var firstSplit = firstLine
                .Split(' ')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            var frame = int.Parse(firstSplit[0].Split(':').Last());
            var pts = double.Parse(firstSplit[1].Split(':').Last());
            var ptsTime = double.Parse(firstSplit[2].Split(':').Last());

            var secondSplit = secondLine
                .Split('=')
                .Last();

            var level = secondSplit == "-inf"
                ? double.MinValue
                : double.Parse(secondSplit);

            return new LoudnessInfo
            {
                Frame = frame,
                Pts = pts,
                PtsTime = ptsTime,
                Level = level
            };
        }
    }
}
