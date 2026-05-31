using CubeTrainer.Cube.Analyzer.Models;

namespace CubeTrainer.Cube.CostTuner;

internal static class StepTokenizer
{
    public static List<string> TokenizeSteps(IEnumerable<AnalysisStep> steps)
    {
        return steps.Select(TokenizeStep).ToList();
    }

    private static string TokenizeStep(AnalysisStep step)
    {
        return step switch
        {
            MotionAnalysisStep motion => $"motion:{motion.Move}:{motion.Hand}:{motion.MotionType}",
            RegripAnalysisStep regrip => $"regrip:{regrip.Hand}:{regrip.NewHandOffset}",
            RotationAnalysisStep rotation => $"rotation:{rotation.Move}",
            _ => step.GetType().Name,
        };
    }
}
