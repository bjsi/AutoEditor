using AutoEditor.Editor;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AutoEditor
{
    class Program
    {
        static async Task Main(string[] args)
        {

            var ffmpeg = new FileInfo(@"C:\Users\james\AppData\Local\Programs\Python\Python38\Scripts\ffmpeg.exe");
            var ffprobe = new FileInfo(@"C:\Users\james\AppData\Local\Programs\Python\Python38\Scripts\ffprobe.exe");
            var testFile = new FileInfo(@"C:\Users\james\Desktop\AudioFiles\BabyElephantWalk60.wav");
            var editor = new VideoEditor(ffmpeg, ffprobe, testFile);

            var res = await editor.GetAudioLoudnessInfo();

        }
    }
}
