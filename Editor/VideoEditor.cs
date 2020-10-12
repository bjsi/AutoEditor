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
                .WithArguments(args)
                .ExecuteBufferedAsync();
        }

        private async Task<int> GetAudioSamplingRate()
        {
            try
            {
                var args = new string[]
                {
                    "-show_streams",
                    videoFile.FullName
                };

                var output = await ExecuteFfprobeCommandAsync(args);
                var info = ParseFfmpegSamplingRate(output.StandardOutput);
                return info?.First().sample_rate ?? -1;

            }
            catch (CommandExecutionException ex)
            {
                Console.WriteLine("Failed to get audio sampling rate info because the call to ffmpeg failed.");
            }
        }

        private List<FFprobeStream> ParseFfmpegSamplingRate(string input)
        {
            return input?.Deserialize<List<FFprobeStream>>();
        }

        /// <summary>
        /// https://superuser.com/questions/1183663/determining-audio-level-peaks-with-ffmpeg
        /// </summary>
        /// <returns></returns>
        private async Task<Dictionary<double, double>> GetAudioLoudnessInfo()
        {
            try
            {
                var reset = await GetAudioSamplingRate() / 1000;

                var file = Path.GetTempFileName();

                var args = new string[]
                {
                    $"-i {videoFile.FullName} ",
                    $"-af astats=metadata=1:reset={reset},ametadata=print:key=lavfi.astats.Overall.RMS_level:file={file} -f null ",
                    "-f null -"
                };

                var info = await ExecuteFfmpegCommandAsync(args);
                var lines = await File.ReadAllLinesAsync(file);
                return ParseFfmpegLoudnessInfo(lines);

            }
            catch (CommandExecutionException e)
            {
                Console.WriteLine("Failed to get audio loudness info because the call to ffmpeg failed.");
            }
            catch (IOException e)
            {
                Console.WriteLine("Failed to get audio loudness info because the call to ffmpeg failed.");
            }
        }

        /// <summary>
        /// Parse the standard output of ffmpeg call.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private Dictionary<double, double> ParseFfmpegLoudnessInfo(string[] input)
        {
            var ret = new Dictionary<double, double>();
            if (input.Length < 2 || input.Length % 2 != 0) 
            {
                throw new InvalidDataException("Unexpected input to ParseFfmpegLoudnessInfo");
            }

            for (int i = 0; i < input.Length; i += 2)
            {
                var firstLine = input[i - 1];
                var secondLine = input[i - 2]
            }
            return ret;
        }
    }
}
