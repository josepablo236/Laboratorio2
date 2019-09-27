using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.IO;
using Laboratorio1.ArbolHuffman;
using Laboratorio1.Controllers;
using Laboratorio1.CompresionLZ;

namespace Laboratorio1.Controllers
{
    public class ReadTextController : Controller
    {
        List<NodoHuffman> listadeNodos = new List<NodoHuffman>();
        public string FilePath = "";
        const int bufferLength = 32000;

        // GET: ReadText
        public ActionResult Index()
        {
            return View();
        }

        //Guardaremos la letra y cuantas veces se repite
        public Dictionary<string, int> Diccionario_CaracteresHuff = new Dictionary<string, int>();
        //Recibo los datos de FileUploadController

        //-------------------COMPRESION DE HUFFMAN -------------------------------------
        public ViewResult Read(string filename)
        {

            List<string> Text_archivo = new List<string>();
            var path = Path.Combine(Server.MapPath("~/Archivo"), filename);
            FilePath = Server.MapPath("~/Archivo");
            using (var stream = new FileStream(path, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    var byteBuffer = new byte[bufferLength];
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        byteBuffer = reader.ReadBytes(bufferLength);
                    }
                    foreach (var item in byteBuffer)
                    {
                        if (Diccionario_CaracteresHuff.ContainsKey(Convert.ToString(item)) == true)
                        {
                            Diccionario_CaracteresHuff[Convert.ToString(item)] += 1;
                        }
                        else
                        {
                            Diccionario_CaracteresHuff.Add(Convert.ToString(item), 1);
                        }
                        Text_archivo.Add(Convert.ToString(item));
                    }
                }
            }
            ArbolHuff arbol = new ArbolHuff();
            //Manda a llamar el metodo del arbol en el que agrega a una lista de nodos, los distintos caracteres que existen
            arbol.agregarNodos(Diccionario_CaracteresHuff, Text_archivo, listadeNodos, FilePath);
            var items = FilesUploaded();
            return View(items);
        }


        //---------------------------------------------------------- COMPRESION LZW  --------------------------------------------------------------------------------------------- -
        public ViewResult ReadLZ(string filename)
        {
            Dictionary<string, int> Diccionario_Caracteres_LZ = new Dictionary<string, int>();
            Dictionary<string, int> Diccionario_CaracteresOriginales = new Dictionary<string, int>();
            List<string> Text_archivo = new List<string>();
            var path = Path.Combine(Server.MapPath("~/Archivo"), filename);
            FilePath = Server.MapPath("~/Archivo");
            using (var stream = new FileStream(path, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    var byteBuffer = new byte[bufferLength];
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        byteBuffer = reader.ReadBytes(bufferLength);
                    }
                    var i = 0;
                    foreach (var item in byteBuffer)
                    {
                        Text_archivo.Add(item.ToString());
                    }
                }
            }
            int newindex = 0;
            int j = 1;
            foreach (var item in Text_archivo)
            {
                if (Diccionario_Caracteres_LZ.ContainsKey(item) == false)
                {
                    Diccionario_Caracteres_LZ.Add(item, j);
                    Diccionario_CaracteresOriginales.Add(item, j);
                    j++;
                }
            }

            //Crear diccionario con letras que ya existen 
            for (int i = newindex; i < Text_archivo.Count - 1; i++)
            {
                i = newindex;
                if (newindex < Text_archivo.Count - 1)
                {
                    if (Diccionario_Caracteres_LZ.ContainsKey(Text_archivo[i]))
                    {
                        newindex = CrearDiccionario(Diccionario_Caracteres_LZ, Text_archivo, Text_archivo[i] + Text_archivo[i + 1], newindex);
                    }
                    else
                    {
                        Diccionario_Caracteres_LZ.Add(Text_archivo[i], Diccionario_Caracteres_LZ.Count + 1);
                        j++;
                        newindex++;
                    }
                }
                else if (newindex == Text_archivo.Count)
                {
                    if (Diccionario_Caracteres_LZ.ContainsKey(Text_archivo[i]) == false)
                    {
                        Diccionario_Caracteres_LZ.Add(Text_archivo[i], Diccionario_Caracteres_LZ.Count + 1);
                        j++;
                    }
                }
            }
            CompresionLZW compresionLZW = new CompresionLZW();
            compresionLZW.ComprimiryEscribir(Diccionario_CaracteresOriginales, Diccionario_Caracteres_LZ, FilePath);
            var items = FilesUploaded2();
            return View(items);
        }

        //De manera recursiva valida si ya existe toma al de la par y vuelve a validar, hasta que no exista
        public int CrearDiccionario(Dictionary<string, int> diccionario, List<string> Texto, string ItemDicctionary, int newindex)
        {


            if (diccionario.ContainsKey(ItemDicctionary))
            {
                CrearDiccionario(diccionario, Texto, ItemDicctionary + Texto[newindex + 2], newindex);

                return newindex += 2;
            }
            else
            {
                diccionario.Add(ItemDicctionary, diccionario.Count + 1); //Perame un rato solo voy a cenar dame 15 mins
                return newindex + 1;
            }
        }

        //------------------- MOSTRAR ARCHIVOS COMPRIMIDOS HUFFMAN-------------------------------------
        private List<string> FilesUploaded()
        {
            var dir = new System.IO.DirectoryInfo(Server.MapPath("~/Archivo"));
            //Unicamente tome los archivos de text, ahorita lo puse como doc para probar pero al final lo podriamos dejar como .txt
            System.IO.FileInfo[] fileNames1 = dir.GetFiles("*.huff");
            //Creo una lista con los nombres de todos los archivos para luego poder mostrarlos
            List<string> filesupld = new List<string>();
            foreach (var file1 in fileNames1)
            {
                filesupld.Add(file1.Name);
            }
            //Devuelvo la lista
            return filesupld;
        }

        //------------------- MOSTRAR ARCHIVOS COMPRIMIDOS LZW -------------------------------------
        private List<string> FilesUploaded2()
        {
            var dir = new System.IO.DirectoryInfo(Server.MapPath("~/Archivo"));
            //Unicamente tome los archivos de text, ahorita lo puse como doc para probar pero al final lo podriamos dejar como .txt
            System.IO.FileInfo[] fileNames2 = dir.GetFiles("*.lzw");
            //Creo una lista con los nombres de todos los archivos para luego poder mostrarlos
            List<string> filesupld = new List<string>();
            foreach (var file2 in fileNames2)
            {
                filesupld.Add(file2.Name);
            }
            //Devuelvo la lista
            return filesupld;
        }

        // Descomprimir
        public void Descomprimir(string TxtName)
        {
            string filepath = FilePath = Server.MapPath("~/Archivo");
            Descompresion descomprimir = new Descompresion();
            descomprimir.LeerArchivo(TxtName, FilePath);
        }


        public FileResult Download(string TxtName)
        {
            var FileVirtualPath = "Archivo/" + "ArchivoComprimido.huff";
            return File(FileVirtualPath, "application/force- download", Path.GetFileName(FileVirtualPath));
        }
    }
}


