using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bayesian_Network
{
    class DataProcessor
    {
        public List<string>[] columns;
        public List<string[]> rows;

        public DataProcessor()
        {
            columns = new List<string>[7];

            for (int i = 0; i < columns.Length; i++)
            {
                columns[i] = new List<string>();
            }

            rows = new List<string[]>();
        }

        //verileri girilen dosya yolundan çeken metod;
        public void GetData(string file)
        {
            //veri yapıları temizlenir;
            rows.Clear();
            for (int i = 0; i < columns.Length; i++)
            {
                columns[i].Clear();
            }

            string path = Directory.GetCurrentDirectory() + "\\" + file;

            try
            {
                string line;
                var streamReader = new StreamReader(path);
                line = streamReader.ReadLine();

                while (line != null)
                {
                    string[] row = line.Split(',');

                    if (!row[0].Equals("Price"))
                    {
                        rows.Add(row);

                        for (int i = 0; i < row.Length; i++)//satırdaki değerler ilgili "column" listesine eklenir
                        {
                            columns[i].Add(row[i]);
                        }
                    }

                    line = streamReader.ReadLine();
                }
                streamReader.Close();
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
