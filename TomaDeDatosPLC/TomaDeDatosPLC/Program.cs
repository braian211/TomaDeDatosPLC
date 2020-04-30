using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TomaDeDatosPLC
{
    class Program
    {
        

        static void Main(string[] args)
        {

            List<Archivo2> archivo2 = new List<Archivo2>();
            Task tarea1 = Task.Factory.StartNew(() =>
            {

                string path = @"c:\archivo2.csv";


                try
                {
                    StreamReader readFile = new StreamReader(path);
                    readFile.ReadLine();
                    while (readFile.Peek() != -1)
                    {
                        string linea = readFile.ReadLine();
                        string[] columna = linea.Split(new Char[] { '-', ';' });
                        Archivo2 archivoTemp = new Archivo2(int.Parse(columna[0]), int.Parse(columna[1]), int.Parse(columna[2]));
                        archivo2.Add(archivoTemp);
                    }
                    readFile.Close();
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message + " fallo la tarea1 ");
                }

            });

            List<Archivo1aY1b> archivo = new List<Archivo1aY1b>();
            

            Task tarea2 = Task.Factory.StartNew(() => {

                string[] path = new string[] { @"c:\archivo1a.csv", @"c:\archivo1b.csv" };
                StreamReader readFile = null;
                foreach (string i in path)
                {
                    try
                    {
                        readFile = new StreamReader(i);
                        readFile.ReadLine();
                        while (readFile.Peek() != -1)
                        {
                            string linea = readFile.ReadLine();
                            string[] columna = linea.Split(new Char[] { ';', '|' });
                            Archivo1aY1b archivoTemp = new Archivo1aY1b(int.Parse(columna[0]), int.Parse(columna[1]) + int.Parse(columna[2]) + int.Parse(columna[3]));
                            archivo.Add(archivoTemp);

                        }
                        readFile.Close();
                        List<Archivo1aY1b> archivoTemp2 = new List<Archivo1aY1b>();
                        for (int x = 0; x < archivo.Count; x++)
                        {
                            for (int z = x+1; z < archivo.Count; z++)
                            {
                                if (archivo[x].Codigo == archivo[z].Codigo)
                                {
                                    archivoTemp2.Add(new Archivo1aY1b(archivo[x].Codigo, archivo[z].Flag + archivo[x].Flag));

                                }
                            }
                        }
                        for(int x = 0; x < archivo.Count ; x++)
                        {
                            for(int z = 0; z < archivoTemp2.Count; z++)
                            {
                                if(archivo[x].Codigo.Equals(archivoTemp2[z].Codigo))
                                {
                                    archivo.RemoveAt(x);
                                }
                            }
                        }
                        archivo.AddRange(archivoTemp2);

                        for (int x = 0; x < archivo.Count; x++)
                        {
                            for (int z = x+1; z < archivo.Count; z++)
                            {
                                if (archivo[x].Codigo.Equals(archivo[z].Codigo))
                                {
                                    archivo.RemoveAt(x);
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message + "fallo la tarea2 ");
                    }
                }
            });

            Task.WaitAll(tarea1, tarea2);

            archivo.Sort((x, y) => x.Codigo.CompareTo(y.Codigo));

            Task tarea3 = Task.Factory.StartNew( () => {

                for (int i = 0; i < archivo.Count; i++)
                {
                    for (int j = 0; j < archivo2.Count; j++) {

                            if (archivo[i].Flag >= archivo2[j].ValorInicial && archivo[i].Flag <= archivo2[j].ValorFinal)
                            {

                                if (archivo2[j].Coeficiente == 0)
                                {
                                archivo[i].Resultado = 0;
                                }
                                else
                                {
                                   archivo[i].Resultado  = archivo[i].Flag / archivo2[j].Coeficiente;
                                }
                            }
                    }
                }
                foreach (Archivo1aY1b x in archivo)
                {
                    x.imprimirFinal();
                }

            });

            tarea3.Wait();
            Console.WriteLine("press key to exit...");
            Console.ReadLine();
                


        }
        class Archivo2
        {
            int valorInicial;
            int coeficiente;
            int valorFinal;

            public Archivo2(int valorInicial, int valorFinal, int coeficiente)
            {
                this.valorInicial = valorInicial;
                this.valorFinal = valorFinal;
                this.coeficiente = coeficiente;
            }
            public void imprimir()
            {
                Console.WriteLine("valor inicial: " + this.valorInicial + "" +
                    " valor final: " + this.valorFinal + " coeficiente: " + this.coeficiente);
            }
            public int ValorInicial { get { return valorInicial; } set { valorInicial = value; } }
            public int Coeficiente { get { return coeficiente; } set { coeficiente = value; } }
            public int ValorFinal { get { return valorFinal; } set { valorFinal = value; } }

        }


        class Archivo1aY1b
        {
            int codigo;
            int flag;
            float resultado;

            public Archivo1aY1b(int codigo, int flag)
            {
                this.codigo = codigo;
                this.flag = flag;

            }
            public Archivo1aY1b(int codigo, int flag, float resultado)
            {
                this.codigo = codigo;
                this.flag = flag;
                this.resultado = resultado;
            }

            public void imprimir()
            {
                Console.WriteLine("codigo: " + this.codigo + " flag: " + this.flag);
            }

            public void imprimirFinal()
            {
                Console.WriteLine("codigo: " + this.codigo + " flag: " + this.flag + " cociente: " + this.resultado);
            }

            public int Codigo { get { return codigo; } set { codigo = value; } }
            public int Flag { get { return flag; } set { flag = value; } }
            public float Resultado { get { return resultado; } set { resultado = value; } }
        }
    }    
}
