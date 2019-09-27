using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Laboratorio1.Controllers;

namespace Laboratorio1.CompresionLZ
{
    public class CompresionLZW
    {
        public double EPSILON { get; private set; }
        public object Covert { get; private set; }
        const int bufferLength = 8;
        const int bufferLeerLength = 1000;
        string TextoCodificado;

        public void ComprimiryEscribir(Dictionary<string, int> Diccionario_CaracteresOriginales, Dictionary<string, int> Diccionario_Caracteres_LZ, string FilePath)
        {
            foreach (var item in Diccionario_Caracteres_LZ.Values)
            {
                TextoCodificado += item;
            }

            var path = Path.Combine(FilePath, "ArchivoComprimido.LZW");
            using (var writeStream1 = new FileStream(path, FileMode.OpenOrCreate))
            {
                using (var writer = new BinaryWriter(writeStream1))
                {
                    foreach (var item in TextoCodificado)
                    {
                        writer.Write(Convert.ToByte(item));
                    }
                    writer.Close();
                }
                writeStream1.Close();
            }
            using (var stream = new FileStream(path, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    using (var writer2 = new StreamWriter(stream))
                    {
                        var byteBufferLeer = new byte[bufferLeerLength];
                        while (reader.BaseStream.Position != reader.BaseStream.Length)
                        {
                            byteBufferLeer = reader.ReadBytes(bufferLeerLength);
                        }
                        if (reader.BaseStream.Position == reader.BaseStream.Length)
                        {
                            writer2.Write("||");
                            foreach (var item in Diccionario_CaracteresOriginales)
                            {
                                writer2.Write(item);
                            }
                        }
                    }
                }
            }
        }
    }
}