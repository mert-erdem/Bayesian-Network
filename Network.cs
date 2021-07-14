using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bayesian_Network
{
    class Network
    {
        private List<Attribute> network;

        public Network(List<string>[] columns)
        {
            //ağ oluşturulur;
            var overallScore = new Attribute("Overall Score", 6, columns[6].Distinct().ToList(), new Attribute[] { });
            var luggageSize = new Attribute("Luggage Size", 4, columns[4].Distinct().ToList(), new Attribute[] { overallScore });
            var safety = new Attribute("Safety", 5, columns[5].Distinct().ToList(), new Attribute[] { overallScore, luggageSize });
            var price = new Attribute("Price", 0, columns[0].Distinct().ToList(), new Attribute[] { overallScore, safety });
            var maintenanceCosts = new Attribute("Maintenance Costs", 1, columns[1].Distinct().ToList(), new Attribute[] { overallScore, safety });
            var seatingCapacity = new Attribute("Seating Capacity", 3, columns[3].Distinct().ToList(), new Attribute[] { overallScore, safety });
            var doors = new Attribute("Doors", 2, columns[2].Distinct().ToList(), new Attribute[] { overallScore, seatingCapacity });

            Attribute[] attributes = { overallScore, luggageSize, safety, price, maintenanceCosts, seatingCapacity, doors };

            network = attributes.ToList();
        }

        //Koşullu olasılık tablolarını oluşturan metod
        public void CreateTables(List<string[]> inputs)
        {
            double pay = 0;
            double payda = 0;
            double probability;

            foreach (var attribute in network)//ağdaki tüm özellikler gezilir;
            {
                switch (attribute.parents.Length)
                {
                    case 2://2 adet parent'ı var ise

                        foreach (var value in attribute.values)//özelliğin tüm değerleri için;
                        {
                            var cptColumn = new Dictionary<string, double>();//cpt tablosunun sütunu (attribute'un her bir değeri için)

                            foreach (var valueP1 in attribute.parents[0].values)//parent1'in her bir değeri için;
                            {
                                foreach (var valueP2 in attribute.parents[1].values)//parent2'nin tüm değerleri;
                                {
                                    foreach (var input in inputs)//tüm train kümesi gezilir;
                                    {
                                        //input'ta parent'ların değerleri varsa
                                        if (input[attribute.parents[0].index].Equals(valueP1) && input[attribute.parents[1].index].Equals(valueP2))
                                        {
                                            payda++;

                                            //mevcut attribute'un değeri varsa
                                            if (input[attribute.index].Equals(value))
                                            {
                                                pay++;
                                            }
                                        }
                                    }

                                    if (payda != 0)
                                    {
                                        probability = pay / payda;
                                    }
                                    else//NaN'ı engellemek için bu paydaya sahip değerlere eşit olasılık değerleri atandı; 
                                    {
                                        probability = (double) 1 / attribute.values.Count;
                                    }

                                    //bulunan değer, ilgili attribute'un koşullu olasılık tablosunun ilgili sütununa parent'larının değerleri ile
                                    //oluşuturulan bir key ile eklenir;
                                    cptColumn.Add(valueP1 + valueP2, probability);//key = parent1'in değeri + parent2'in değeri

                                    pay = 0;
                                    payda = 0;
                                }
                            }

                            //oluşturulan bu sütun koşullu olasılık tablosuna eklenir;
                            attribute.cpt.Add(value, cptColumn);//key = attribute'un değeri
                        }

                        break;

                    case 1://1 parent'ı varsa

                        foreach (var value in attribute.values)
                        {
                            var cptColumn = new Dictionary<string, double>();

                            foreach (var valueP in attribute.parents[0].values)
                            {
                                foreach (var input in inputs)
                                {
                                    if (input[attribute.parents[0].index].Equals(valueP))
                                    {
                                        payda++;

                                        if (input[attribute.index].Equals(value))
                                            pay++;
                                    }
                                }

                                if (payda != 0)
                                {
                                    probability = pay / payda;
                                }
                                else
                                {
                                    probability = (double) 1 / attribute.values.Count;
                                }

                                cptColumn.Add(valueP, probability);

                                pay = 0;
                                payda = 0;
                            }

                            attribute.cpt.Add(value, cptColumn);
                        }


                        break;

                    case 0://parent'ı yoksa

                        foreach (var value in attribute.values)
                        {
                            var cptColumn = new Dictionary<string, double>();

                            foreach (var input in inputs)
                            {
                                if (input[attribute.index].Equals(value))
                                    pay++;
                            }

                            probability = pay / inputs.Count;

                            cptColumn.Add(value, probability);

                            pay = 0;
                            payda = 0;

                            attribute.cpt.Add(value, cptColumn);
                        }

                        break;
                }
            }

            //tabloları konsola yazdırma;
            ShowTables();
        }

        //her bir kayıt için P(OverallScore = good | testkaydı) bulan metod
        public void Prediction(List<string[]> inputs)
        {
            double bad = 0, good = 0;
            double TN = 0, TP = 0;
            int index = 0;

            foreach (var input in inputs)//her bir input için;
            {
                //gelen test kaydına göre Overall Score' un "good" olma olasılığı ( P(OverallScore=good | testkaydı) ) ;

                var pay = new List<double>();
                var payda = new List<double>();

                //P(testkaydı | OverallScore = good) * P(OverallScore = good)
                FindProbabilities("good", pay, input);

                //P(testkaydı | OverallScore = bad) * P(OverallScore = bad)
                FindProbabilities("bad", payda, input);

                //P(OverallScore = good | testkaydı)
                double goodProbability = 0;

                double paySonuc = 1;
                foreach (var probability in pay)//pay değerleri çarpılır
                {
                    paySonuc *= probability;
                }

                double paydaSonuc = 1;
                foreach (var probability in payda)//payda değerleri çarpılır
                {
                    paydaSonuc *= probability;
                }

                //P(testkaydı)
                paydaSonuc += paySonuc;

                //gelen test kaydına göre Overall Score' un "good" olma olasılığı ( P(OverallScore = good | testkaydı) );
                goodProbability = paySonuc / paydaSonuc;

                if (input[input.Length - 1].Equals("bad"))
                {
                    bad++;//toplam bad sayısı
                }
                else
                {
                    good++;//toplam good sayısı
                }

                if (1 - goodProbability > goodProbability)//bad olma olasılığı good olma olasılığından büyük ise
                {
                    if (input[input.Length - 1].Equals("bad"))//true negative
                    {
                        TN++;
                        if (inputs.Count == 728)
                            Console.WriteLine($"Orijinal: bad   Tahmin edilen: bad   Olasilik = {Math.Round(1 - goodProbability, 3)}");
                    }
                    else if (inputs.Count == 728)
                    {
                        Console.WriteLine($"Orijinal: good   Tahmin edilen: bad   Olasilik = {Math.Round(1 - goodProbability, 3)}");
                    }
                }
                else if (1 - goodProbability < goodProbability)
                {
                    if (input[input.Length - 1].Equals("good"))//true positive
                    {
                        TP++;
                        if (inputs.Count == 728)
                            Console.WriteLine($"Orijinal: good   Tahmin edilen: good   Olasilik = {Math.Round(goodProbability, 3)}");
                    }
                    else if (inputs.Count == 728)
                    {
                        Console.WriteLine($"Orijinal: bad   Tahmin edilen: good   Olasilik = {Math.Round(goodProbability, 3)}");
                    }

                }

                //Console.WriteLine($"P(OverallScore=bad|testkaydi) {index+1} = "+Math.Round(1-goodProbability, 3));
                //index++;
            }


            //başarı ölçütlerinin konsola yazdırılması;
            if (inputs.Count == 728)
                Console.WriteLine(Environment.NewLine + "-----Test Kumesi Basari Olcutleri-----" + Environment.NewLine);            
            else
                Console.WriteLine(Environment.NewLine + "-----Egitim Kumesi Basari Olcutleri-----" + Environment.NewLine);

            Console.WriteLine("Toplam Kayit = " + inputs.Count);
            Console.WriteLine("Toplam good = " + good);
            Console.WriteLine("Toplam bad = " + bad);
            Console.WriteLine("TP =  " + TP);
            Console.WriteLine("TN =  " + TN);
            Console.WriteLine("TP Orani = " + Math.Round(TP / good, 3));
            Console.WriteLine("TN Orani = " + Math.Round(TN / bad, 3));
            Console.WriteLine("Accuracy = " + Math.Round((TP + TN) / (good + bad), 3) + Environment.NewLine);
        }

        //verilen bir Overall Score default değerine(good veya bad) göre olasılık değerlerini ilgili tablolardan çekip, bu değerleri
        //"probabilities" listesine ekleyen metod
        private void FindProbabilities(string defaultValue, List<double> probabilities, string[] input)
        {
            //pay/payda hesaplama (her attribute için bir olasılık değeri hesaplanır) P(attribute=input[inputIndex]|parent'larının değerleri(varsa))
            for (int inputIndex = 0; inputIndex < input.Length; inputIndex++)
            {
                //index' e göre attribute bulunur
                var currentAttribute = network.Find(x => x.index == inputIndex);

                string key = "";

                if (currentAttribute.parents.Any())//attribute'un parent'ı var ise
                {
                    foreach (var parent in currentAttribute.parents)//her parent'ı için
                    {
                        if (parent.name.Equals("Overall Score"))
                        {
                            key += defaultValue;//default olarak good ya da bad
                        }
                        else
                        {
                            key += input[parent.index];//parent' ın test kaydındaki değeri
                        }
                    }

                    var column = currentAttribute.cpt[input[currentAttribute.index]];//attribute'un tablosundan ilgili değere ait olan dictionary alınır
                    double probabilityValue = column[key];//toplanan key ile bu dictionary'den olasılık değeri alınır
                    probabilities.Add(probabilityValue);//bu değer "probabilities" listesine eklenir
                }
                else//parent'ı yok ise Overall Score'dur
                {
                    key = defaultValue;//verilen default değer

                    var column = currentAttribute.cpt[key];
                    double probabilityValue = column[key];
                    probabilities.Add(probabilityValue);
                }
            }
        }

        private void ShowTables()
        {
            foreach (var attribute in network)
            {
                Console.WriteLine(attribute.name);
                Console.WriteLine("----------------" + Environment.NewLine);
                if (attribute.cpt != null)
                {
                    foreach (var value in attribute.cpt)//dizilerin dizisi
                    {

                        foreach (var deger in value.Value)//dizi
                        {
                            Console.Write(value.Key + " " + deger.Key + "-" + Math.Round(deger.Value, 3) + "     ");
                        }
                        Console.WriteLine(Environment.NewLine);
                    }
                    Console.WriteLine(Environment.NewLine);
                }
            }
        }
    }
}
