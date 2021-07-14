using System;

namespace Bayesian_Network
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataProcessor = new DataProcessor();
            //train kümesi çekilir;
            dataProcessor.GetData("Final-Train.txt");

            //ağ oluşturulur;
            var network = new Network(dataProcessor.columns);

            //tablolar oluşturulur;
            network.CreateTables(dataProcessor.rows);
            //train kümesi için tahminleme;
            //network.Prediction(dataProcessor.rows);

            //test kümesi çekilir;
            dataProcessor.GetData("Final-Test.txt");
            //test kümesi için tahminleme;
            network.Prediction(dataProcessor.rows);
        }
    }
}
