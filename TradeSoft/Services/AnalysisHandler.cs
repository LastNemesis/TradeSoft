using TradeSoft.Models;

namespace TradeSoft.Services
{
    public class AnalysisHandler
    {
        private Dictionary<int, Analysis> _strategiesAnalysis = new Dictionary<int, Analysis>();
        private Logger _logger;

        public AnalysisHandler(List<int> stratgiesId, Logger logger) {
            _logger  = logger;
            foreach (var strategyId in stratgiesId)
            {
                _strategiesAnalysis[strategyId] = new Analysis(100000);
            }
        }

        public void AnalyseExecutionBit(ExecutionBit executionBit)
        {
            _strategiesAnalysis[executionBit.Id].executed = executionBit;
            _strategiesAnalysis[executionBit.Id].runMethods();

            string analysisResults = _strategiesAnalysis[executionBit.Id].ToString();

            _logger.LogAnalysis(executionBit, analysisResults);
        }

        public void AnalyseExecutionBit(object sender, ExecutionBit e)
        {
            AnalyseExecutionBit(e);
        }
    }
}
