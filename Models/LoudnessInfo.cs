namespace AutoEditor.Models
{
    public class LoudnessInfo
    {
        public LoudnessInfo(int frame, double pts, double ptsTime, double level)
        {
            Frame = frame;
            Pts = pts;
            PtsTime = ptsTime;
            Level = level;
        }

        public LoudnessInfo() { }

        public int Frame { get; set; }
        public double Pts { get; set; }
        public double PtsTime { get; set; }
        public double Level { get; set; }

    }
}
