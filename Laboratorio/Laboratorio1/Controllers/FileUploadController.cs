using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

namespace Laboratorio1.Controllers
{
    public class FileUploadController : Controller
    {
        // GET: FileUpload
        public ActionResult Index()
        {
            //La vista que me va a mostrar todos los archivos que ya se han subido
            var items = FilesUploaded();
            return View(items);
        }

        //----------------------------SUBIR ARCHIVO ------------------------------------------------------------------------
        //El metodo que mando a llamar desde el Index.cshtml al momento de presionar el submit ("Upload File")

        [HttpPost]          //Recibo un archivo
        public ActionResult Index(HttpPostedFileBase file)
        {
            //Valido que no sea nulo y que contenga texto, ya que valido que el archivo pese.
            if (file != null && file.ContentLength > 0)
                try
                {
                    //Valido que unicamente puedan cargar archivos de text
                    if (Path.GetExtension(file.FileName) == ".txt")
                    {
                        //Me va a devolver la ruta en la que se encuentra la carpeta "Archivos" 
                        string path = Path.Combine(Server.MapPath("~/Archivo"),
                                      //Toma el nombre del archivo
                                      Path.GetFileName(file.FileName));
                        //Entonces path, va a ser igual a la ruta +  el nombre del archivo
                        file.SaveAs(path); //Guarda el archivo en la carpeta "Archivos"
                        ViewBag.Message = "File uploaded";
                    }

                }
                catch
                {
                    ViewBag.Message = "ERROR. Try uploading other file";
                }
            else
            {
                ViewBag.Message = "Please upload a file";
            }

            var items = FilesUploaded();
            return View(items);
        }

        private List<string> FilesUploaded()
        {
            var dir = new System.IO.DirectoryInfo(Server.MapPath("~/Archivo"));
            //Unicamente tome los archivos de text, ahorita lo puse como doc para probar pero al final lo podriamos dejar como .txt
            System.IO.FileInfo[] fileNames = dir.GetFiles("*.txt");
            //Creo una lista con los nombres de todos los archivos para luego poder mostrarlos
            List<string> filesupld = new List<string>();
            foreach (var file in fileNames)
            {
                filesupld.Add(file.Name);
            }
            //Devuelvo la lista
            return filesupld;
        }

        // Este lo vamos a usar luego que ya podamos descomprimir jajaja

        public ActionResult Read(string TxtName)
        {

            return RedirectToAction("Read", "ReadText", new { filename = TxtName });

        }
        public ActionResult ReadLZ(string TxtName)
        {

            return RedirectToAction("ReadLZ", "ReadText", new { filename = TxtName });

        }
    }
}