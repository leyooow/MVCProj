using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.IO;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;
using MVCProj.Models;

namespace MVCProj.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Leo()
        {
            ViewBag.Message = "Upload a PDF only.";

            return View(GetFiles());
        }


        [HttpPost]
        public JsonResult GetPDF(int fileId)
        {
            byte[] bytes;
            string fileName, contentType;
            string constr = ConfigurationManager.ConnectionStrings["SAMMYBOI"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "SELECT Name, Data, content_type FROM pdf_files WHERE Id=@Id";
                    cmd.Parameters.AddWithValue("@Id", fileId);
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        sdr.Read();
                        bytes = (byte[])sdr["Data"];
                        contentType = sdr["Content_Type"].ToString();
                        fileName = sdr["Name"].ToString();
                    }
                    con.Close();
                }
            }
            JsonResult jsonResult = Json(new { FileName = fileName, ContentType = contentType, Data = bytes });
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }


        [HttpPost]
        public ActionResult Leo(HttpPostedFileBase postedFile)
        {
            byte[] bytes;
            using (BinaryReader br = new BinaryReader(postedFile.InputStream))
            {
                bytes = br.ReadBytes(postedFile.ContentLength);
            }
            string constr = ConfigurationManager.ConnectionStrings["SAMMYBOI"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "INSERT INTO pdf_files (name, content_type, data) VALUES (@Name, @ContentType, @Data)";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@Name", Path.GetFileName(postedFile.FileName));
                    cmd.Parameters.AddWithValue("@ContentType", postedFile.ContentType);
                    cmd.Parameters.AddWithValue("@Data", bytes);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

            return View(GetFiles());
        }

        private static List<Pdf> GetFiles()
        {
            List<Pdf> files = new List<Pdf>();
            string constr = ConfigurationManager.ConnectionStrings["SAMMYBOI"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT Id, Name FROM pdf_files"))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            files.Add(new Pdf
                            {
                                Id = Convert.ToInt32(sdr["Id"]),
                                Name = sdr["Name"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }
            return files;
        }

        [HttpPost]
        public FileResult DownloadFile(int? fileId)
        {
            byte[] bytes;
            string fileName, contentType;
            string constr = ConfigurationManager.ConnectionStrings["SAMMYBOI"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "SELECT Name, Data, Content_type FROM pdf_files WHERE Id=@Id";
                    cmd.Parameters.AddWithValue("@Id", fileId);
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        sdr.Read();
                        bytes = (byte[])sdr["Data"];
                        contentType = sdr["Content_type"].ToString();
                        fileName = sdr["Name"].ToString();
                    }
                    con.Close();
                }
            }

            return File(bytes, contentType, fileName);
        }

        
    }
}