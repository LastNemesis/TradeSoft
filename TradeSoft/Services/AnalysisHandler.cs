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

        public void analyseExecutionBits(List<Order> executedOrders)
        {
            foreach (var order in executedOrders)
            {
                //const String analysis = _strategiesAnalysis[order.StratId].runMethods();
                //_logger.LogAnalysis(order, analysis);
            }
        }
    }
}
