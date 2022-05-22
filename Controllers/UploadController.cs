using IP_KPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Data.OleDb;
using System.Linq;
using System.Threading.Tasks;
using IP_KPI.Data;
using ExcelDataReader;
using System.Data;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using MoreLinq;

namespace IP_KPI.Controllers
{
    public class UploadController : Controller
    {
        private readonly db_a7baa5_ipkpiContext _db;

        public UploadController(db_a7baa5_ipkpiContext db)
        {

            _db = db;
        }
        [Authorize(Roles = "Manager,Data Entry")]
        public ActionResult Index()
        {
            return View();
        }




        //switch method to direct to the appropriate method
        public IActionResult UploadExcel(String kpi, IFormFile file)
        {

            switch (kpi)
            {
                case "survey":
                    UploadSurvey(file);
                    break;
                case "publication":
                    UploadPublication(file);
                    break;
                case "facultystatistics":
                    UploadFacultyStatistic(file);
                    break;
                case "AlumniEmployment":
                    UploadAlmuniEmployment(file);
                    break;
                case "BatchStatistic":
                    UploadBatchStatistic(file);
                    break;
                case "ClassSection":
                    UploadClassSection(file);
                    break;
            }

            return View("Index");
        }

        //switch method to direct to the appropriate method 
        public IActionResult Replace(int rep)
        {
            switch (rep)
            {
                case 1:
                    ReplaceSurvey();
                    break;
                case 2:
                    ReplacePublication();
                    break;
                case 3:
                    ReplaceFacultyStatistic();
                    break;
                case 4:
                    ReplaceAlmuniEmployment();
                    break;
                case 5:
                    ReplaceBatchStatistic();
                    break;
                case 6:
                    ReplaceClassSection();
                    break;


            }

            return View("Index");
        }




        //#######   Survey   #######//
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public IActionResult UploadSurvey(IFormFile file)
        {
            var SSlist = new List<Survey>();
            var KPlist = new List<Kpiprogram>();

            using (var stream = new MemoryStream())
            {
                try
                {
                    file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowcount = worksheet.Dimension.Rows;

                        //Go through all the rows in the excel sheet and store them in the list
                        var check = worksheet.Cells[1, 11].Value.ToString().Trim();

                        if (!check.Equals("اعداد الطلبه المسجلين"))
                        {
                            ViewBag.err = " الملف لا يطابق المؤشر المختار";
                            return RedirectToPage("Index");
                        }
                        for (int row = 2; row <= rowcount; row++)
                        {

                            var ProName = worksheet.Cells[row, 3].Value.ToString().Trim();
                            var ProLevel = worksheet.Cells[row, 4].Value.ToString().Trim();
                            var proID = _db.UniPrograms.Where(x => x.ProgramName == ProName && x.Level == ProLevel).Select(x => x.ProgramId).SingleOrDefault();
                            var KPI_code = worksheet.Cells[row, 18].Value.ToString().Trim();
                            var kpi_id = _db.Kpis.Where(x => x.Kpicode == KPI_code).Select(x => x.KpiId).SingleOrDefault();
                            var surveyScore = Convert.ToDouble(worksheet.Cells[row, 13].Value.ToString().Trim());
                            var numOfRespondent = Convert.ToInt32(worksheet.Cells[row, 12].Value.ToString().Trim());
                            if(numOfRespondent == 0)
                            {
                                ViewBag.err = "حدث خطأ! اعداد المجيبين على الاستبانة يساوي صفر بينما مجموع التقييم على مستوى البرنامج لا يساوي صفر";
                                return RedirectToPage("Index");
                            }

                            SSlist.Add(new Survey
                            {

                                Year = worksheet.Cells[row, 5].Value.ToString().Trim(),
                                Term = worksheet.Cells[row, 6].Value.ToString().Trim(),
                                Gender = worksheet.Cells[row, 7].Value.ToString().Trim(),
                                Nationality = worksheet.Cells[row, 8].Value.ToString().Trim(),
                                ExtraActivity = worksheet.Cells[row, 9].Value.ToString().Trim(),
                                StudentCase = worksheet.Cells[row, 10].Value.ToString().Trim(),
                                NumberOfStudent = Convert.ToInt32(worksheet.Cells[row, 11].Value.ToString().Trim()),
                                NumOfRespondent = numOfRespondent,
                                SurveyScore = surveyScore,
                                NumOfWeakScore = Convert.ToInt32(worksheet.Cells[row, 14].Value.ToString().Trim()),
                                NumOfNeutralScore = Convert.ToInt32(worksheet.Cells[row, 15].Value.ToString().Trim()),
                                NumOfGoodScore = Convert.ToInt32(worksheet.Cells[row, 16].Value.ToString().Trim()),
                                NumOfExcellentScore = Convert.ToInt32(worksheet.Cells[row, 17].Value.ToString().Trim()),
                                KpiId = kpi_id,
                                ProgramId = proID
                            });

                            KPlist.Add(new Kpiprogram
                            {
                                Year = worksheet.Cells[row, 5].Value.ToString().Trim(),
                                Term = worksheet.Cells[row, 6].Value.ToString().Trim(),
                                TargetBenchmark = Convert.ToDouble(worksheet.Cells[row, 19].Value.ToString().Trim()),
                                InternalBenchmark = Convert.ToDouble(worksheet.Cells[row, 20].Value.ToString().Trim()),
                                ExternalBenchmark = Convert.ToDouble(worksheet.Cells[row, 21].Value.ToString().Trim()),
                                ActualKpivalue = surveyScore/numOfRespondent,
                                KpiId = kpi_id,
                                ProgramId = proID,
                                Gender = worksheet.Cells[row, 7].Value.ToString().Trim()

                            });

                        }

                    }
                    //check if the excel is empty 
                    if (SSlist.Count == 0 && KPlist.Count == 0)
                    {
                        ViewBag.err = " الملف فارغ يرجى تعبئة الملف وإعادة رفعه";
                        return RedirectToPage("Index");
                    }

                    //Because the values in a year for the same term and gender will be the same, it should only be added once.
                    var uniqueItems = KPlist.DistinctBy(m => new { m.ProgramId, m.Gender, m.Term, m.KpiId, m.Year }).ToList();


                    var newSSlist = new List<Survey>();
                    var newKPlist = new List<Kpiprogram>();

                    //check if a record already exist in the database , if yes delete from the list 
                    foreach (var n in SSlist.ToList())
                    {

                        bool compare = _db.Surveys.Where(x => x.ProgramId == n.ProgramId && x.Year == n.Year && x.Term == n.Term && x.KpiId == n.KpiId && x.Gender == n.Gender && x.Nationality == n.Nationality && x.StudentCase == n.StudentCase).Any();
                        if (compare)
                        {
                            newSSlist.Add(n);
                            SSlist.Remove(n);
                        }

                    }

                    foreach (var n in uniqueItems.ToList())
                    {

                        var compare = _db.Kpiprograms.Where(x => x.ProgramId == n.ProgramId && x.Year == n.Year && x.Term == n.Term && x.KpiId == n.KpiId && x.Gender == n.Gender).Any();
                        if (compare)
                        {
                            newKPlist.Add(n);
                            uniqueItems.Remove(n);
                        }

                    }

                    //if the lists are empty
                    /*   if (KPlist.Count == 0 && SSlist.Count == 0)
                       {
                           ViewBag.err = "البيانات مضافة مسبقًا";
                           return RedirectToPage("Index");
                       }*/

                    //add data to the database
                    foreach (var n in SSlist)
                    {
                        _db.Surveys.Add(n);
                    }

                    _db.SaveChanges();

                    foreach (var n in uniqueItems)
                    {
                        _db.Kpiprograms.Add(n);
                    }
                    _db.SaveChanges();

                    //foreach (var n in SSlist)
                    //{
                    //    UpdateStudentsOrGradsCount(Convert.ToInt32(n.ProgramId), n.Gender, n.Year, n.Term);
                    //}

                    if (newKPlist.Count > 0 && newSSlist.Count > 0)
                    {
                        ViewBag.replace = 1;
                        TempData["RKPlist"] = JsonConvert.SerializeObject(newKPlist);
                        TempData["RSSlist"] = JsonConvert.SerializeObject(newSSlist);

                        return View("Index");
                    }

                    ViewBag.suc = "تم حفظ البيانات بنجاح";
                    return RedirectToPage("Index");
                }
                catch (NullReferenceException e)
                {
                    ViewBag.err = "حدث خطأ اثناء استرجاع البيانات من ملف الاكسل، الرجاء التحقق من صحة التعبئة";
                    return RedirectToPage("Index");
                }

                catch (SqlException e)
                {
                    ViewBag.err = " لم يتم حفظ البيانات في قاعدة البيانات الرجاء اعادة المحاولة";
                    return RedirectToPage("Index");

                }
                catch (Exception e)
                {
                    ViewBag.err = " حدث خطأ ما، الرجاء التحقق واعادة التحميل";
                    return RedirectToPage("Index");

                }
            }
        }

        //replace student survey
        public IActionResult ReplaceSurvey()
        {

            var listSS = JsonConvert.DeserializeObject<List<Survey>>(TempData["RSSlist"].ToString());
            var listKP = JsonConvert.DeserializeObject<List<Kpiprogram>>(TempData["RKPlist"].ToString());

            foreach (var n in listSS)
            {
                var compare = _db.Surveys.Where(x => x.ProgramId == n.ProgramId && x.Year == n.Year && x.Term == n.Term && x.KpiId == n.KpiId && x.Gender == n.Gender && x.Nationality == n.Nationality && x.StudentCase == n.StudentCase).FirstOrDefault();
                _db.Surveys.Remove(compare);
                _db.Surveys.Add(n);
            }
            foreach (var n in listKP)
            {

                var compare = _db.Kpiprograms.Where(x => x.ProgramId == n.ProgramId && x.Year == n.Year && x.Term == n.Term && x.KpiId == n.KpiId && x.Gender == n.Gender).FirstOrDefault();
                _db.Kpiprograms.Remove(compare);
                _db.Kpiprograms.Add(n);
            }

            _db.SaveChanges();

            return View("Index");

        }




        //#######   Publication Report   #######//
        [HttpPost]
        [ValidateAntiForgeryToken()]      
        public IActionResult UploadPublication(IFormFile file)
        {
            var PublicationList = new List<PublicationReport>();
            var KPIList = new List<Kpiprogram>();

            using (var stream = new MemoryStream())
            {
                try
                {
                    file.CopyToAsync(stream);

                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowcount = worksheet.Dimension.Rows;
                        //check if it the right file 
                        var head = worksheet.Cells[1, 7].Value.ToString().Trim();
                        if (!head.Equals("اعداد منشورات هيئة التدريس بدوام كامل او مايعادله"))
                        {
                            ViewBag.err = " الملف لا يطابق المؤشر المختار";
                            return RedirectToPage("Index");
                        }
                        //add Publication reprot  

                        for (int row = 2; row <= rowcount; row++)
                        {
                            var pname = worksheet.Cells[row, 3].Value.ToString().Trim();
                            var programlevel = worksheet.Cells[row, 5].Value.ToString().Trim();
                            var pn = _db.UniPrograms.Where(x => x.ProgramName == pname && x.Level == programlevel).Select(x => x.ProgramId).SingleOrDefault();
                            var gender = worksheet.Cells[row, 6].Value.ToString().Trim();
                            var numOfFacultyOneP = Convert.ToInt32(worksheet.Cells[row, 9].Value.ToString().Trim());
                            var numOfPublications = Convert.ToInt32(worksheet.Cells[row, 7].Value.ToString().Trim());
                            var numOfCitations = Convert.ToInt32(worksheet.Cells[row, 8].Value.ToString().Trim());
                            var year = worksheet.Cells[row, 4].Value.ToString().Trim();
                            var facultyStatisticsList = _db.FacultyStatistics.Where(x => x.ProgramId == pn && x.Year == year && x.Gender == gender).ToList();
                            var numOfFaculty = 0;
                            foreach (var faculty in facultyStatisticsList)
                                numOfFaculty += (int)(faculty.NumOfAssistantProf + faculty.NumOfAssociateProf + faculty.NumOfLecturer + faculty.NumOfProf + faculty.NumOfTeacher + faculty.NumOfTeachingAssistant);
                            if (numOfFaculty == 0)
                            {
                                ViewBag.err = "حدث خطأ! اعداد اعضاء هيئة التدريس في هذه السنه يساوي صفر بينما اعداد المنشورات للاعضاء في هذه السنه لا تساوي صفر، الرجاء التاكد من تحميل ملف مؤشرات احصاءات هيئة التعليم اولا";
                                return RedirectToPage("Index");
                            }
                            if (numOfPublications == 0)
                            {
                                ViewBag.err = "حدث خطأ! اعداد منشورات اعضاء هيئة التدريس في هذه السنه يساوي صفر بينما اعداد الاقتباسات للمنشورات في هذه السنه لا تساوي صفر";
                                return RedirectToPage("Index");
                            }

                            PublicationList.Add(new PublicationReport
                            {
                                Gender = gender,
                                NumOfPublications = numOfPublications,
                                NumOfCitations = numOfCitations,
                                NumOfFacultyOneP = numOfFacultyOneP,
                                Year = year,
                                ProgramId = pn
                            });

                            //add KPI-P-14 in KPIprogram table 
                            KPIList.Add(new Kpiprogram
                            {
                                KpiId = _db.Kpis.Where(x => x.Kpicode == "KPI-P-14").Select(x => x.KpiId).SingleOrDefault(),
                                ProgramId = pn,
                                TargetBenchmark = Convert.ToDouble(worksheet.Cells[row, 10].Value.ToString().Trim()),
                                InternalBenchmark = Convert.ToDouble(worksheet.Cells[row, 11].Value.ToString().Trim()),
                                ExternalBenchmark = Convert.ToDouble(worksheet.Cells[row, 12].Value.ToString().Trim()),
                                ActualKpivalue = ((float)numOfFacultyOneP / (float)numOfFaculty)* 100,
                                Year = year,
                                Gender = gender
                            });

                            //add KPI-P-15 in KPIprogram table 
                            KPIList.Add(new Kpiprogram
                            {
                                KpiId = _db.Kpis.Where(x => x.Kpicode == "KPI-P-15").Select(x => x.KpiId).SingleOrDefault(),
                                ProgramId = pn,
                                TargetBenchmark = Convert.ToDouble(worksheet.Cells[row, 13].Value.ToString().Trim()),
                                InternalBenchmark = Convert.ToDouble(worksheet.Cells[row, 14].Value.ToString().Trim()),
                                ExternalBenchmark = Convert.ToDouble(worksheet.Cells[row, 15].Value.ToString().Trim()),
                                ActualKpivalue = ((float)numOfPublications / (float)numOfFaculty)*100,
                                Year = year,
                                Gender = gender
                            });

                            //add KPI-P-16 in KPIprogram table 

                            KPIList.Add(new Kpiprogram
                            {
                                KpiId = _db.Kpis.Where(x => x.Kpicode == "KPI-P-16").Select(x => x.KpiId).SingleOrDefault(),
                                ProgramId = pn,
                                TargetBenchmark = Convert.ToDouble(worksheet.Cells[row, 16].Value.ToString().Trim()),
                                InternalBenchmark = Convert.ToDouble(worksheet.Cells[row, 17].Value.ToString().Trim()),
                                ExternalBenchmark = Convert.ToDouble(worksheet.Cells[row, 18].Value.ToString().Trim()),
                                ActualKpivalue = ((float)numOfCitations / (float)numOfPublications)*100,
                                Year = year,
                                Gender = gender
                            });



                        }
                    }
                    //check if the excel is empty 
                    if (PublicationList.Count == 0 && KPIList.Count == 0)
                    {
                        ViewBag.err = " الملف فارغ يرجى تعبئة الملف وإعادة رفعه";
                        return RedirectToPage("Index");
                    }

                    //duplicates in the same list, Because the values in a year for the same term and gender will be the same, it should only be added once.
                    var uniqueKPI = KPIList.DistinctBy(m => new { m.ProgramId, m.Gender, m.KpiId, m.Year }).ToList();
                    var uniquePublication = PublicationList.DistinctBy(m => new { m.ProgramId, m.Gender, m.Year }).ToList();

                    //remove the duplication from publication 
                    var PublicationListDup = new List<PublicationReport>();

                    foreach (var f in uniquePublication.ToList())
                    {
                        var compare = _db.PublicationReports.Where(x => x.ProgramId == f.ProgramId && x.Year == f.Year && x.Gender == f.Gender).Any();
                        if (compare)
                        {
                            PublicationListDup.Add(f);
                            uniquePublication.Remove(f);
                        }
                    }

                    //remove the duplication from KPIProgram 
                    var kpistatlist = new List<Kpiprogram>();
                    foreach (var n in uniqueKPI.ToList())
                    {
                        var compare = _db.Kpiprograms.Where(x => x.ProgramId == n.ProgramId && x.Year == n.Year && x.KpiId == n.KpiId && x.Gender == n.Gender).Any();
                        if (compare)
                        {
                            kpistatlist.Add(n);
                            uniqueKPI.Remove(n);
                        }
                    }
                    foreach (var n in uniquePublication)
                    {
                        _db.PublicationReports.Add(n);
                    }

                    _db.SaveChanges();
                    foreach (var n in uniqueKPI)
                    {
                        _db.Kpiprograms.Add(n);
                    }
                    _db.SaveChanges();
                    if (kpistatlist.Count > 0 && PublicationListDup.Count > 0)
                    {
                        ViewBag.replace = 2;
                        TempData["kpilumlist"] = JsonConvert.SerializeObject(kpistatlist);
                        TempData["PublicationListDup"] = JsonConvert.SerializeObject(PublicationListDup);

                        return RedirectToPage("Index");
                    }

                    ViewBag.suc = "تم حفظ البيانات بنجاح";
                    return RedirectToPage("Index");
                }
                catch (NullReferenceException e)
                {
                    ViewBag.err = "حدث خطأ اثناء استرجاع البيانات من ملف الاكسل، الرجاء التحقق من صحة التعبئة";
                    return RedirectToPage("Index");
                }
                catch (SqlException e)
                {
                    ViewBag.err = " لم يتم حفظ البيانات في قاعدة البيانات الرجاء اعادة المحاولة";
                    return RedirectToPage("Index");
                }
                catch (Exception e)
                {
                    ViewBag.err = " حدث خطأ ما، الرجاء التحقق واعادة التحميل";
                    return RedirectToPage("Index");
                }
            }
        }

        // replace PublicationReport // not working yet //
        public IActionResult ReplacePublication()
        {

            var PublicationListDup = JsonConvert.DeserializeObject<List<PublicationReport>>(TempData["PublicationListDup"].ToString());
            var kpistatlist = JsonConvert.DeserializeObject<List<Kpiprogram>>(TempData["kpilumlist"].ToString());

            foreach (var f in PublicationListDup)
            {
                var compare = _db.PublicationReports.Where(x => x.ProgramId == f.ProgramId && x.Year == f.Year && x.Gender == f.Gender).FirstOrDefault();
                _db.PublicationReports.Remove(compare);
                _db.PublicationReports.Add(f);
            }
            foreach (var n in kpistatlist)
            {

                var compare = _db.Kpiprograms.Where(x => x.ProgramId == n.ProgramId && x.Year == n.Year && x.KpiId == n.KpiId && x.Gender == n.Gender).FirstOrDefault();
                _db.Kpiprograms.Remove(compare);
                _db.Kpiprograms.Add(n);
            }

            _db.SaveChanges();

            return View("Index");


        }




        //#######   Faculty Statistic   #######//
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public IActionResult UploadFacultyStatistic(IFormFile file)
        {

            var FacultyStatisticList = new List<FacultyStatistic>();
            var KPIList = new List<Kpiprogram>();

            using (var stream = new MemoryStream())
            {
                try
                {
                    file.CopyToAsync(stream);

                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowcount = worksheet.Dimension.Rows;
                        //check if it the right file 
                        var head = worksheet.Cells[1, 7].Value.ToString().Trim();
                        if (!head.Equals("اعداد المدرسين"))
                        {
                            ViewBag.err = " الملف لا يطابق المؤشر المختار";
                            return RedirectToPage("Index");
                        }
                        //add  facultyStatistic 

                        for (int row = 2; row <= rowcount; row++)
                        {
                            var pname = worksheet.Cells[row, 3].Value.ToString().Trim();
                            var programlevel = worksheet.Cells[row, 5].Value.ToString().Trim();
                            var pn = _db.UniPrograms.Where(x => x.ProgramName == pname && x.Level == programlevel).Select(x => x.ProgramId).SingleOrDefault();
                            var year = worksheet.Cells[row, 4].Value.ToString().Trim();
                            var gender = worksheet.Cells[row, 6].Value.ToString().Trim();
                            var type = worksheet.Cells[row, 14].Value.ToString().Trim();
                            var numOfTeacher = Convert.ToInt32(worksheet.Cells[row, 7].Value.ToString().Trim());
                            var numOfTeachingAssistant = Convert.ToInt32(worksheet.Cells[row, 8].Value.ToString().Trim());
                            var numOfLecturer = Convert.ToInt32(worksheet.Cells[row, 9].Value.ToString().Trim());
                            var numOfAssistantProf = Convert.ToInt32(worksheet.Cells[row, 10].Value.ToString().Trim());
                            var numOfAssociateProf = Convert.ToInt32(worksheet.Cells[row, 11].Value.ToString().Trim());
                            var numOfProf = Convert.ToInt32(worksheet.Cells[row, 12].Value.ToString().Trim());
                            var numOfResignation = Convert.ToInt32(worksheet.Cells[row, 13].Value.ToString().Trim());

                            var KPI_11_Score = 0.0; var onJobEmployee = 0.0; float studentCount = 0;
                            if (type == "على راس العمل")
                            {
                                var kpiStudent = _db.Kpis.FirstOrDefault(x => x.Kpicode == "KPI-P-03");
                                var studentList = _db.Surveys.Where(x => x.KpiId == kpiStudent.KpiId && x.ProgramId == pn && x.Year == year && x.Gender == gender).ToList();
                                foreach (var item in studentList)
                                    studentCount += (float)item.NumberOfStudent;
                                if (studentCount == 0)
                                {
                                    ViewBag.err = "حدث خطأ! اعداد الطلبة في هذه السنه يساوي صفر، الرجاء التأكد من تحميل ملف مؤشرات الاستبانات اولا";
                                    return RedirectToPage("Index");
                                }
                                onJobEmployee = numOfAssistantProf + numOfAssociateProf + numOfLecturer + numOfProf + numOfTeacher + numOfTeachingAssistant;
                                KPI_11_Score = studentCount / onJobEmployee;

                            }
                            var KPI_12_Score = numOfAssistantProf + numOfAssociateProf + numOfLecturer + numOfProf + numOfTeacher + numOfTeachingAssistant;
                            var KPI_13_Score = ((float)numOfResignation / (float)(numOfAssistantProf + numOfAssociateProf + numOfLecturer + numOfProf + numOfTeacher + numOfTeachingAssistant)) * 100;

                           


                            FacultyStatisticList.Add(new FacultyStatistic
                            {
                                Gender = gender,
                                NumOfTeacher = numOfTeacher,
                                NumOfTeachingAssistant = numOfTeachingAssistant,
                                NumOfLecturer = numOfLecturer,
                                NumOfAssistantProf = numOfAssistantProf,
                                NumOfAssociateProf = numOfAssociateProf,
                                NumOfProf = numOfProf,
                                NumOfResignation = numOfResignation,
                                Type = type,
                                Year = year,
                                NumOfRespondent = Convert.ToInt32(worksheet.Cells[row, 15].Value.ToString().Trim()),
                                SurveyScore = Convert.ToInt32(worksheet.Cells[row, 16].Value.ToString().Trim()),
                                NumOfWeakScore = Convert.ToInt32(worksheet.Cells[row, 17].Value.ToString().Trim()),
                                NumOfNeutralScore = Convert.ToInt32(worksheet.Cells[row, 18].Value.ToString().Trim()),
                                NumOfGoodScore = Convert.ToInt32(worksheet.Cells[row, 19].Value.ToString().Trim()),
                                NumOfExellentScore = Convert.ToInt32(worksheet.Cells[row, 20].Value.ToString().Trim()),
                                ProgramId = pn

                            });


                            //add KPI-P-11 in KPIprogram table 
                            KPIList.Add(new Kpiprogram
                            {
                                KpiId = _db.Kpis.Where(x => x.Kpicode == "KPI-P-11").Select(x => x.KpiId).SingleOrDefault(),
                                ProgramId = pn,
                                TargetBenchmark = Convert.ToDouble(worksheet.Cells[row, 21].Value.ToString().Trim()),
                                InternalBenchmark = Convert.ToDouble(worksheet.Cells[row, 22].Value.ToString().Trim()),
                                ExternalBenchmark = Convert.ToDouble(worksheet.Cells[row, 23].Value.ToString().Trim()),
                                ActualKpivalue = (int)KPI_11_Score,
                                Year = year,
                                Gender = gender
                            });

                            //add KPI-P-12 in KPIprogram table 

                            KPIList.Add(new Kpiprogram
                            {
                                KpiId = _db.Kpis.Where(x => x.Kpicode == "KPI-P-12").Select(x => x.KpiId).SingleOrDefault(),
                                ProgramId = pn,
                                TargetBenchmark = Convert.ToDouble(worksheet.Cells[row, 24].Value.ToString().Trim()),
                                InternalBenchmark = Convert.ToDouble(worksheet.Cells[row, 25].Value.ToString().Trim()),
                                ExternalBenchmark = Convert.ToDouble(worksheet.Cells[row, 26].Value.ToString().Trim()),
                                ActualKpivalue = KPI_12_Score,
                                Year = year,
                                Gender = gender
                            });

                            //add KPI-P-13 in KPIprogram table 
                            KPIList.Add(new Kpiprogram
                            {
                                KpiId = _db.Kpis.Where(x => x.Kpicode == "KPI-P-13").Select(x => x.KpiId).SingleOrDefault(),
                                ProgramId = pn,
                                TargetBenchmark = Convert.ToDouble(worksheet.Cells[row, 27].Value.ToString().Trim()),
                                InternalBenchmark = Convert.ToDouble(worksheet.Cells[row, 28].Value.ToString().Trim()),
                                ExternalBenchmark = Convert.ToDouble(worksheet.Cells[row, 29].Value.ToString().Trim()),
                                ActualKpivalue = KPI_13_Score,
                                Year = year,
                                Gender = gender


                            });

                        }
                    }
                    //check if the excel is empty 
                    if (FacultyStatisticList.Count == 0 && KPIList.Count == 0)
                    {
                        ViewBag.err = " الملف فارغ يرجى تعبئة الملف وإعادة رفعه";
                        return RedirectToPage("Index");
                    }

                    //duplicates in the same list, Because the values in a year for the same term and gender will be the same, it should only be added once.

                    var uniqueFaculty = FacultyStatisticList.DistinctBy(m => new { m.ProgramId, m.Gender, m.Year, m.Type }).ToList();
                    var uniqueKPI = KPIList.DistinctBy(m => new { m.ProgramId, m.Gender, m.KpiId, m.Year }).ToList();


                    //remove the duplication from faculty 
                    var listfstat = new List<FacultyStatistic>();
                    foreach (var f in uniqueFaculty.ToList())
                    {

                        var compare = _db.FacultyStatistics.Where(x => x.ProgramId == f.ProgramId && x.Year == f.Year && x.Gender == f.Gender).Any();
                        if (compare)
                        {
                            listfstat.Add(f);
                            uniqueFaculty.Remove(f);
                        }

                    }

                    //remove the duplication from KPIProgram 
                    var kpistatlist = new List<Kpiprogram>();
                    foreach (var n in uniqueKPI.ToList())
                    {

                        var compare = _db.Kpiprograms.Where(x => x.ProgramId == n.ProgramId && x.Year == n.Year && x.KpiId == n.KpiId && x.Gender == n.Gender).Any();
                        if (compare)
                        {
                            kpistatlist.Add(n);
                            uniqueKPI.Remove(n);
                        }

                    }
                    foreach (var n in uniqueFaculty)
                    {
                        _db.FacultyStatistics.Add(n);
                    }


                    _db.SaveChanges();
                    foreach (var n in uniqueKPI)
                    {
                        _db.Kpiprograms.Add(n);
                    }
                    _db.SaveChanges();
                    if (kpistatlist.Count > 0 && listfstat.Count > 0)
                    {

                        ViewBag.replace = 3;
                        TempData["kpistatlist"] = JsonConvert.SerializeObject(kpistatlist);
                        TempData["listfstat"] = JsonConvert.SerializeObject(listfstat);

                        return RedirectToPage("Index");
                    }


                    ViewBag.suc = "تم حفظ البيانات بنجاح";
                    return RedirectToPage("Index");
                }
                catch (NullReferenceException e)
                {
                    ViewBag.err = "حدث خطأ اثناء استرجاع البيانات من ملف الاكسل، الرجاء التحقق من صحة التعبئة";
                    return RedirectToPage("Index");
                }
                catch (SqlException e)
                {
                    ViewBag.err = " لم يتم حفظ البيانات في قاعدة البيانات الرجاء اعادة المحاولة";
                    return RedirectToPage("Index");

                }
                catch (Exception e)
                {
                    ViewBag.err = " حدث خطأ ما، الرجاء التحقق واعادة التحميل";
                    return RedirectToPage("Index");

                }
            }
        }
        //Replace Faculty Statistic
        public IActionResult ReplaceFacultyStatistic()
        {

            var listfstat = JsonConvert.DeserializeObject<List<FacultyStatistic>>(TempData["listfstat"].ToString());
            var kpistatlist = JsonConvert.DeserializeObject<List<Kpiprogram>>(TempData["kpistatlist"].ToString());

            foreach (var f in listfstat)
            {
                var compare = _db.FacultyStatistics.Where(x => x.ProgramId == f.ProgramId && x.Year == f.Year && x.Gender == f.Gender).FirstOrDefault();
                _db.FacultyStatistics.Remove(compare);
                _db.FacultyStatistics.Add(f);
            }
            foreach (var n in kpistatlist)
            {

                var compare = _db.Kpiprograms.Where(x => x.ProgramId == n.ProgramId && x.Year == n.Year && x.KpiId == n.KpiId && x.Gender == n.Gender).FirstOrDefault();
                _db.Kpiprograms.Remove(compare);
                _db.Kpiprograms.Add(n);
            }

            _db.SaveChanges();

            return View("Index");

        }

       


        //#######   Alumni and employment   #######//
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public IActionResult UploadAlmuniEmployment(IFormFile file)
        {
            var AlmuniList = new List<AlumniEmployment>();
            var KPIList = new List<Kpiprogram>();


            using (var stream = new MemoryStream())
            {
                try
                {
                    file.CopyToAsync(stream);

                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowcount = worksheet.Dimension.Rows;
                        //check if it the right file 
                        var head = worksheet.Cells[1, 7].Value.ToString().Trim();
                        if (!head.Equals("عدد الخريجين الموظفين في اول سنة من تخرجهم"))
                        {
                            ViewBag.err = " الملف لا يطابق المؤشر المختار";
                            return RedirectToPage("Index");
                        }
                        //add  Alumni  

                        for (int row = 2; row <= rowcount; row++)
                        {
                            var pname = worksheet.Cells[row, 3].Value.ToString().Trim();
                            var programlevel = worksheet.Cells[row, 5].Value.ToString().Trim();
                            var pn = _db.UniPrograms.Where(x => x.ProgramName == pname && x.Level == programlevel).Select(x => x.ProgramId).SingleOrDefault();
                            var totalGradEvalByCompany = Convert.ToDouble(worksheet.Cells[row, 11].Value.ToString().Trim());
                            var numOfCompanyEvaled = Convert.ToInt32(worksheet.Cells[row, 12].Value.ToString().Trim());
                            var gender = worksheet.Cells[row, 6].Value.ToString().Trim();
                            var year = worksheet.Cells[row, 4].Value.ToString().Trim();
                            var kpi = _db.Kpis.FirstOrDefault(x => x.Kpicode == "KPI-P-02");
                            var studentList = _db.Surveys.Where(x => x.ProgramId == pn && x.Gender == gender && x.Year == year && x.KpiId == kpi.KpiId).ToList();
                            int countOfGrads = 0;
                            foreach (var item in studentList)
                                countOfGrads += (int)item.NumberOfStudent;
                            var gradEmployed = Convert.ToInt32(worksheet.Cells[row, 7].Value.ToString().Trim());
                            var gradEnrolled = Convert.ToInt32(worksheet.Cells[row, 8].Value.ToString().Trim());
                            var numOfStudentPassQiyasExam = Convert.ToInt32(worksheet.Cells[row, 9].Value.ToString().Trim());
                            var numOfStudentPassCareerExam = Convert.ToInt32(worksheet.Cells[row, 10].Value.ToString().Trim());

                            if (countOfGrads == 0)
                            {
                                ViewBag.err = "حدث خطأ! اعداد الخريجين في هذه السنه يساوي صفر بينما اعداد الخصائص للخريجين في هذه السنه لا تساوي صفر، الرجاء التاكد من تحميل ملف مؤشرات الاستبانات اولا";
                                return RedirectToPage("Index");
                            }
                            if (numOfCompanyEvaled == 0)
                            {
                                ViewBag.err = "حدث خطأ! اعداد جهات العمل المقيمة للخريجين في هذه السنه تساوي صفر بينما مجموع التقييمات لجهات العمل في هذه السنه لا تساوي صفر،";
                                return RedirectToPage("Index");
                            }

                            AlmuniList.Add(new AlumniEmployment
                            {
                                Gender = gender,
                                GradEmployed = gradEmployed,
                                GradEnrolled = gradEnrolled,
                                NumOfStudentPassQiyasExam = numOfStudentPassQiyasExam,
                                NumOfStudentPassCareerExam = numOfStudentPassCareerExam,
                                TotalGradEvalByCompany = totalGradEvalByCompany,
                                NumOfCompanyEvaled = numOfCompanyEvaled,
                                Year = year,
                                ProgramId = pn
                            });

                            //add KPI-P-06-A in KPIprogram table 
                            KPIList.Add(new Kpiprogram
                            {

                                KpiId = _db.Kpis.Where(x => x.Kpicode == "KPI-P-06").Select(x => x.KpiId).SingleOrDefault(),
                                ProgramId = pn,
                                TargetBenchmark = Convert.ToDouble(worksheet.Cells[row, 13].Value.ToString().Trim()),
                                InternalBenchmark = Convert.ToDouble(worksheet.Cells[row, 14].Value.ToString().Trim()),
                                ExternalBenchmark = Convert.ToDouble(worksheet.Cells[row, 15].Value.ToString().Trim()),
                                ActualKpivalue = ((float)numOfStudentPassQiyasExam / (float)countOfGrads) * 100,
                                Year = worksheet.Cells[row, 4].Value.ToString().Trim(),
                                Gender = worksheet.Cells[row, 6].Value.ToString().Trim(),
                            });

                            //add KPI-P-06-B in KPIprogram table 
                            KPIList.Add(new Kpiprogram
                            {
                                KpiId = _db.Kpis.Where(x => x.Kpicode == "KPI-P-06-B").Select(x => x.KpiId).SingleOrDefault(),
                                ProgramId = pn,
                                TargetBenchmark = Convert.ToDouble(worksheet.Cells[row, 16].Value.ToString().Trim()),
                                InternalBenchmark = Convert.ToDouble(worksheet.Cells[row, 17].Value.ToString().Trim()),
                                ExternalBenchmark = Convert.ToDouble(worksheet.Cells[row, 18].Value.ToString().Trim()),
                                ActualKpivalue = ((float)numOfStudentPassCareerExam / (float)countOfGrads)*100,
                                Year = worksheet.Cells[row, 4].Value.ToString().Trim(),
                                Gender = worksheet.Cells[row, 6].Value.ToString().Trim()
                            });

                            //add KPI-P-07 in KPIprogram table 

                            KPIList.Add(new Kpiprogram
                            {
                                KpiId = _db.Kpis.Where(x => x.Kpicode == "KPI-P-07").Select(x => x.KpiId).SingleOrDefault(),
                                ProgramId = pn,
                                TargetBenchmark = Convert.ToDouble(worksheet.Cells[row, 19].Value.ToString().Trim()),
                                InternalBenchmark = Convert.ToDouble(worksheet.Cells[row, 20].Value.ToString().Trim()),
                                ExternalBenchmark = Convert.ToDouble(worksheet.Cells[row, 21].Value.ToString().Trim()),
                                ActualKpivalue = ((float)(gradEmployed + gradEnrolled) / (float)countOfGrads)*100,
                                Year = worksheet.Cells[row, 4].Value.ToString().Trim(),
                                Gender = worksheet.Cells[row, 6].Value.ToString().Trim()
                            });

                            //add KPI-P-09 in KPIprogram table 
                            KPIList.Add(new Kpiprogram
                            {
                                KpiId = _db.Kpis.Where(x => x.Kpicode == "KPI-P-09").Select(x => x.KpiId).SingleOrDefault(),
                                ProgramId = pn,
                                TargetBenchmark = Convert.ToDouble(worksheet.Cells[row, 22].Value.ToString().Trim()),
                                InternalBenchmark = Convert.ToDouble(worksheet.Cells[row, 23].Value.ToString().Trim()),
                                ExternalBenchmark = Convert.ToDouble(worksheet.Cells[row, 24].Value.ToString().Trim()),
                                ActualKpivalue = (float)totalGradEvalByCompany / (float)numOfCompanyEvaled,
                                Year = worksheet.Cells[row, 4].Value.ToString().Trim(),
                                Gender = worksheet.Cells[row, 6].Value.ToString().Trim()
                            });
                        }
                    }
                    //check if the excel is empty 
                    if (AlmuniList.Count == 0 && KPIList.Count == 0)
                    {
                        ViewBag.err = " الملف فارغ يرجى تعبئة الملف وإعادة رفعه";
                        return RedirectToPage("Index");
                    }
                    //duplicates in the same list, Because the values in a year for the same term and gender will be the same, it should only be added once.
                    var uniqueKPI = KPIList.DistinctBy(m => new { m.ProgramId, m.Gender, m.KpiId, m.Year }).ToList();
                    var uniqueAlmuni = AlmuniList.DistinctBy(m => new { m.ProgramId, m.Gender, m.Year }).ToList();

                    //remove the duplication from faculty 
                    var AlmuniListDup = new List<AlumniEmployment>();

                    foreach (var f in uniqueAlmuni.ToList())
                    {
                        var compare = _db.AlumniEmployments.Where(x => x.ProgramId == f.ProgramId && x.Year == f.Year && x.Gender == f.Gender).Any();
                        if (compare)
                        {
                            AlmuniListDup.Add(f);
                            uniqueAlmuni.Remove(f);
                        }
                    }

                    //remove the duplication from KPIProgram 
                    var kpistatlist = new List<Kpiprogram>();
                    foreach (var n in uniqueKPI.ToList())
                    {
                        var compare = _db.Kpiprograms.Where(x => x.ProgramId == n.ProgramId && x.Year == n.Year && x.KpiId == n.KpiId && x.Gender == n.Gender).Any();
                        if (compare)
                        {
                            kpistatlist.Add(n);
                            uniqueKPI.Remove(n);
                        }

                    }
                    foreach (var n in uniqueAlmuni)
                    {
                        _db.AlumniEmployments.Add(n);
                    }

                    _db.SaveChanges();
                    foreach (var n in uniqueKPI)
                    {
                        _db.Kpiprograms.Add(n);
                    }
                    _db.SaveChanges();
                    if (kpistatlist.Count > 0 && AlmuniListDup.Count > 0)
                    {
                        ViewBag.replace = 4;
                        TempData["kpilumlist"] = JsonConvert.SerializeObject(kpistatlist);
                        TempData["AlmuniListDup"] = JsonConvert.SerializeObject(AlmuniListDup);

                        return RedirectToPage("Index");
                    }

                    ViewBag.suc = "تم حفظ البيانات بنجاح";
                    return RedirectToPage("Index");
                }
                catch (NullReferenceException e)
                {
                    ViewBag.err = "حدث خطأ اثناء استرجاع البيانات من ملف الاكسل، الرجاء التحقق من صحة التعبئة";
                    return RedirectToPage("Index");
                }
                catch (SqlException e)
                {
                    ViewBag.err = " لم يتم حفظ البيانات في قاعدة البيانات الرجاء اعادة المحاولة";
                    return RedirectToPage("Index");
                }
                catch (Exception e)
                {
                    ViewBag.err = " حدث خطأ ما، الرجاء التحقق واعادة التحميل";
                    return RedirectToPage("Index");
                }
            }
        }

        //replace Almuni and Employment
        public IActionResult ReplaceAlmuniEmployment()
        {

            var AlmuniListDup = JsonConvert.DeserializeObject<List<AlumniEmployment>>(TempData["AlmuniListDup"].ToString());
            var kpistatlist = JsonConvert.DeserializeObject<List<Kpiprogram>>(TempData["kpilumlist"].ToString());

            foreach (var f in AlmuniListDup)
            {
                var compare = _db.AlumniEmployments.Where(x => x.ProgramId == f.ProgramId && x.Year == f.Year && x.Gender == f.Gender).FirstOrDefault();
                _db.AlumniEmployments.Remove(compare);
                _db.AlumniEmployments.Add(f);
            }
            foreach (var n in kpistatlist)
            {

                var compare = _db.Kpiprograms.Where(x => x.ProgramId == n.ProgramId && x.Year == n.Year && x.KpiId == n.KpiId && x.Gender == n.Gender).FirstOrDefault();
                _db.Kpiprograms.Remove(compare);
                _db.Kpiprograms.Add(n);
            }

            _db.SaveChanges();

            return View("Index");


        }




        //#######   BatchStatistic   #######//
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public IActionResult UploadBatchStatistic(IFormFile file)
        {
            var BatchList = new List<BatchStatistic>();
            var KPIList = new List<Kpiprogram>();


            using (var stream = new MemoryStream())
            {
                try
                {
                    file.CopyToAsync(stream);

                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowcount = worksheet.Dimension.Rows;
                        //check if it the right file 
                        var head = worksheet.Cells[1, 7].Value.ToString().Trim();
                        if (!head.Equals("عدد الطلاب في السنة التحضيرية"))
                        {
                            ViewBag.err = " الملف لا يطابق المؤشر المختار";
                            return RedirectToPage("Index");
                        }
                        //add  Batch  

                        for (int row = 2; row <= rowcount; row++)
                        {
                            var pname = worksheet.Cells[row, 3].Value.ToString().Trim();
                            var programlevel = worksheet.Cells[row, 5].Value.ToString().Trim();
                            var pn = _db.UniPrograms.Where(x => x.ProgramName == pname && x.Level == programlevel).Select(x => x.ProgramId).SingleOrDefault();
                            var numFirstYearStudent = Convert.ToInt32(worksheet.Cells[row, 7].Value.ToString().Trim());
                            var numOfBatchStudent = Convert.ToInt32(worksheet.Cells[row, 8].Value.ToString().Trim());
                            var numStudentContinue = Convert.ToInt32(worksheet.Cells[row, 9].Value.ToString().Trim());
                            var numMinYearStudent = Convert.ToInt32(worksheet.Cells[row, 10].Value.ToString().Trim());

                            if (numOfBatchStudent == 0)
                            {
                                ViewBag.err = "حدث خطأ! اعداد طلاب الدفعه في هذه السنه يساوي صفر بينما اعداد الطلبة المتخرجين بالحد الادنى من السنوات لهذه الدفعة لا يساوي صفر";
                                return RedirectToPage("Index");
                            }
                            if (numFirstYearStudent == 0)
                            {
                                ViewBag.err = "حدث خطأ! اعداد الطلاب في السنة الاولى يساوي صفر بينما اعداد الطلبة المستمرين للعام الثاني على نفس المسار لا يساوي صفر";
                                return RedirectToPage("Index");
                            }

                            BatchList.Add(new BatchStatistic
                            {
                                Gender = worksheet.Cells[row, 6].Value.ToString().Trim(),
                                NumFirstYearStudent = numFirstYearStudent,
                                NumOfBatchStudent = numOfBatchStudent,
                                NumStudentContinue = numStudentContinue,
                                NumMinYearStudent = numMinYearStudent,
                                Year = worksheet.Cells[row, 4].Value.ToString().Trim(),
                                ProgramId = pn
                            });


                            //add KPI-P-04 in KPIprogram table 
                            KPIList.Add(new Kpiprogram
                            {
                                KpiId = _db.Kpis.Where(x => x.Kpicode == "KPI-P-04").Select(x => x.KpiId).SingleOrDefault(),
                                ProgramId = pn,
                                TargetBenchmark = Convert.ToDouble(worksheet.Cells[row, 11].Value.ToString().Trim()),
                                InternalBenchmark = Convert.ToDouble(worksheet.Cells[row, 12].Value.ToString().Trim()),
                                ExternalBenchmark = Convert.ToDouble(worksheet.Cells[row, 13].Value.ToString().Trim()),
                                ActualKpivalue = ((float)numMinYearStudent / (float)numOfBatchStudent) * 100,
                                Year = worksheet.Cells[row, 4].Value.ToString().Trim(),
                                Gender = worksheet.Cells[row, 6].Value.ToString().Trim()
                            });

                            //add KPI-P-05 in KPIprogram table 

                            KPIList.Add(new Kpiprogram
                            {
                                KpiId = _db.Kpis.Where(x => x.Kpicode == "KPI-P-05").Select(x => x.KpiId).SingleOrDefault(),
                                ProgramId = pn,
                                TargetBenchmark = Convert.ToDouble(worksheet.Cells[row, 14].Value.ToString().Trim()),
                                InternalBenchmark = Convert.ToDouble(worksheet.Cells[row, 15].Value.ToString().Trim()),
                                ExternalBenchmark = Convert.ToDouble(worksheet.Cells[row, 16].Value.ToString().Trim()),
                                ActualKpivalue = ((float)numStudentContinue / (float)numFirstYearStudent)*100,
                                Year = worksheet.Cells[row, 4].Value.ToString().Trim(),
                                Gender = worksheet.Cells[row, 6].Value.ToString().Trim()
                            });
                        }
                    }
                    //check if the excel is empty 
                    if (BatchList.Count == 0 && KPIList.Count == 0)
                    {
                        ViewBag.err = " الملف فارغ يرجى تعبئة الملف وإعادة رفعه";
                        return RedirectToPage("Index");
                    }
                    //duplicates in the same list, Because the values in a year for the same term and gender will be the same, it should only be added once.
                    var uniqueKPI = KPIList.DistinctBy(m => new { m.ProgramId, m.Gender, m.KpiId, m.Year }).ToList();
                    var uniqueBatch = BatchList.DistinctBy(m => new { m.ProgramId, m.Gender, m.Year }).ToList();

                    //remove the duplication from faculty 
                    var BatchListDup = new List<BatchStatistic>();

                    foreach (var f in uniqueBatch.ToList())
                    {
                        var compare = _db.BatchStatistics.Where(x => x.ProgramId == f.ProgramId && x.Year == f.Year && x.Gender == f.Gender).Any();
                        if (compare)
                        {
                            BatchListDup.Add(f);
                            uniqueBatch.Remove(f);
                        }
                    }

                    //remove the duplication from KPIProgram 
                    var kpistatlist = new List<Kpiprogram>();
                    foreach (var n in uniqueKPI.ToList())
                    {

                        var compare = _db.Kpiprograms.Where(x => x.ProgramId == n.ProgramId && x.Year == n.Year && x.Term == n.Term && x.KpiId == n.KpiId && x.Gender == n.Gender).Any();
                        if (compare)
                        {
                            kpistatlist.Add(n);
                            uniqueKPI.Remove(n);
                        }

                    }
                    foreach (var n in uniqueBatch)
                    {
                        _db.BatchStatistics.Add(n);
                    }


                    _db.SaveChanges();
                    foreach (var n in uniqueKPI)
                    {
                        _db.Kpiprograms.Add(n);
                    }
                    _db.SaveChanges();
                    if (kpistatlist.Count > 0 && BatchListDup.Count > 0)
                    {
                        ViewBag.replace = 5;
                        TempData["kpibatchlist"] = JsonConvert.SerializeObject(kpistatlist);
                        TempData["BatchListDup"] = JsonConvert.SerializeObject(BatchListDup);

                        return RedirectToPage("Index");
                    }


                    ViewBag.suc = "تم حفظ البيانات بنجاح";
                    return RedirectToPage("Index");
                }
                catch (NullReferenceException e)
                {
                    ViewBag.err = "حدث خطأ اثناء استرجاع البيانات من ملف الاكسل، الرجاء التحقق من صحة التعبئة";
                    return RedirectToPage("Index");
                }
                catch (SqlException e)
                {
                    ViewBag.err = " لم يتم حفظ البيانات في قاعدة البيانات الرجاء اعادة المحاولة";
                    return RedirectToPage("Index");
                }
                catch (Exception e)
                {
                    ViewBag.err = " حدث خطأ ما، الرجاء التحقق واعادة التحميل";
                    return RedirectToPage("Index");
                }
            }
        }

        //replcae Batch Statistic
        public IActionResult ReplaceBatchStatistic()
        {

            var BatchListDup = JsonConvert.DeserializeObject<List<BatchStatistic>>(TempData["BatchListDup"].ToString());
            var kpistatlist = JsonConvert.DeserializeObject<List<Kpiprogram>>(TempData["kpibatchlist"].ToString());

            foreach (var f in BatchListDup)
            {
                var compare = _db.BatchStatistics.Where(x => x.ProgramId == f.ProgramId && x.Year == f.Year && x.Gender == f.Gender).FirstOrDefault();
                _db.BatchStatistics.Remove(compare);
                _db.BatchStatistics.Add(f);
            }
            foreach (var n in kpistatlist)
            {

                var compare = _db.Kpiprograms.Where(x => x.ProgramId == n.ProgramId && x.Year == n.Year && x.Term == n.Term && x.KpiId == n.KpiId && x.Gender == n.Gender).FirstOrDefault();
                _db.Kpiprograms.Remove(compare);
                _db.Kpiprograms.Add(n);
            }

            _db.SaveChanges();

            return View("Index");


        }




        //#######   ClassSections   #######//
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public IActionResult UploadClassSection(IFormFile file)
        {

            var ClassList = new List<ClassSection>();
            var KPIList = new List<Kpiprogram>();


            using (var stream = new MemoryStream())
            {
                try
                {
                    file.CopyToAsync(stream);

                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowcount = worksheet.Dimension.Rows;
                        //check if it the right file 
                        var head = worksheet.Cells[1, 8].Value.ToString().Trim();
                        if (!head.Equals("عدد الشعب في البرنامج"))
                        {
                            ViewBag.err = " الملف لا يطابق المؤشر المختار";
                            return RedirectToPage("Index");
                        }
                        //add  Batch  

                        for (int row = 2; row <= rowcount; row++)
                        {
                            var pname = worksheet.Cells[row, 3].Value.ToString().Trim();
                            var programlevel = worksheet.Cells[row, 6].Value.ToString().Trim();
                            var pn = _db.UniPrograms.Where(x => x.ProgramName == pname && x.Level == programlevel).Select(x => x.ProgramId).SingleOrDefault();
                            var gender = worksheet.Cells[row, 7].Value.ToString().Trim();
                            var term = worksheet.Cells[row, 5].Value.ToString().Trim();
                            var numOfClasses = Convert.ToInt32(worksheet.Cells[row, 8].Value.ToString().Trim());
                            var year = worksheet.Cells[row, 4].Value.ToString().Trim();
                            var kpi = _db.Kpis.FirstOrDefault(x => x.Kpicode == "KPI-P-03");
                            var studentList = _db.Surveys.Where(x => x.ProgramId == pn && x.Gender == gender && x.Year == year && x.Term == term && x.KpiId == kpi.KpiId).ToList();
                            int countOfStudent = 0;
                            foreach (var item in studentList)
                                countOfStudent += (int)item.NumberOfStudent;

                            if (numOfClasses == 0)
                            {
                                ViewBag.err = "حدث خطأ! اعداد الشعب في هذه السنه يساوي صفر بينما اعداد الطلبة في هذه السنه لا تساوي صفر";
                                return RedirectToPage("Index");
                            }
                            if (countOfStudent == 0)
                            {
                                ViewBag.err = "حدث خطأ! اعداد الطلبة في هذه السنه يساوي صفر، الرجاء التأكد من تحميل ملف مؤشرات الاستبانات اولا";
                                return RedirectToPage("Index");
                            }

                            ClassList.Add(new ClassSection
                            {
                                Gender = gender,
                                Term = term,
                                NumOfClasses = numOfClasses,
                                Year = year,
                                ProgramId = pn
                            });


                            //add KPI-P-08 in KPIprogram table 
                            KPIList.Add(new Kpiprogram
                            {
                                KpiId = _db.Kpis.Where(x => x.Kpicode == "KPI-P-08").Select(x => x.KpiId).SingleOrDefault(),
                                ProgramId = pn,
                                TargetBenchmark = Convert.ToDouble(worksheet.Cells[row, 9].Value.ToString().Trim()),
                                InternalBenchmark = Convert.ToDouble(worksheet.Cells[row, 10].Value.ToString().Trim()),
                                ExternalBenchmark = Convert.ToDouble(worksheet.Cells[row, 11].Value.ToString().Trim()),
                                ActualKpivalue = (float)countOfStudent / (float)numOfClasses,
                                Year = year,
                                Term = term,
                                Gender = gender
                            });
                        }
                    }
                    //check if the excel is empty 
                    if (ClassList.Count == 0 && KPIList.Count == 0)
                    {
                        ViewBag.err = " الملف فارغ يرجى تعبئة الملف وإعادة رفعه";
                        return RedirectToPage("Index");
                    }

                    //duplicates in the same list, Because the values in a year for the same term and gender will be the same, it should only be added once.
                    var uniqueKPI = KPIList.DistinctBy(m => new { m.ProgramId, m.Gender, m.KpiId, m.Year }).ToList();
                    var uniqueClass = ClassList.DistinctBy(m => new { m.ProgramId, m.Gender, m.Term, m.Year }).ToList();

                    //remove the duplication from class 
                    var ClassListDup = new List<ClassSection>();

                    foreach (var f in uniqueClass.ToList())
                    {
                        var compare = _db.ClassSections.Where(x => x.ProgramId == f.ProgramId && x.Year == f.Year && x.Gender == f.Gender && x.Term == f.Term).Any();
                        if (compare)
                        {
                            ClassListDup.Add(f);
                            uniqueClass.Remove(f);
                        }
                    }

                    //remove the duplication from KPIProgram 
                    var kpistatlist = new List<Kpiprogram>();
                    foreach (var n in uniqueKPI.ToList())
                    {
                        var compare = _db.Kpiprograms.Where(x => x.ProgramId == n.ProgramId && x.Year == n.Year && x.Term == n.Term && x.KpiId == n.KpiId && x.Gender == n.Gender).Any();
                        if (compare)
                        {
                            kpistatlist.Add(n);
                            uniqueKPI.Remove(n);
                        }
                    }
                    foreach (var n in uniqueClass)
                    {
                        _db.ClassSections.Add(n);
                    }

                    _db.SaveChanges();
                    foreach (var n in uniqueKPI)
                    {
                        _db.Kpiprograms.Add(n);
                    }
                    _db.SaveChanges();
                    if (kpistatlist.Count > 0 && ClassListDup.Count > 0)
                    {
                        ViewBag.replace = 6;
                        TempData["kpiclasslist"] = JsonConvert.SerializeObject(kpistatlist);
                        TempData["ClassListDup"] = JsonConvert.SerializeObject(ClassListDup);

                        return RedirectToPage("Index");
                    }

                    ViewBag.suc = "تم حفظ البيانات بنجاح";
                    return RedirectToPage("Index");
                }
                catch (NullReferenceException e)
                {
                    ViewBag.err = "حدث خطأ اثناء استرجاع البيانات من ملف الاكسل، الرجاء التحقق من صحة التعبئة";
                    return RedirectToPage("Index");
                }
                catch (SqlException e)
                {
                    ViewBag.err = " لم يتم حفظ البيانات في قاعدة البيانات الرجاء اعادة المحاولة";
                    return RedirectToPage("Index");
                }
                catch (Exception e)
                {
                    ViewBag.err = " حدث خطأ ما، الرجاء التحقق واعادة التحميل";
                    return RedirectToPage("Index");
                }
            }
        }

        //replcae ClassSection
        public IActionResult ReplaceClassSection()
        {

            var ClassListDup = JsonConvert.DeserializeObject<List<ClassSection>>(TempData["ClassListDup"].ToString());
            var kpistatlist = JsonConvert.DeserializeObject<List<Kpiprogram>>(TempData["kpiclasslist"].ToString());

            foreach (var f in ClassListDup)
            {
                var compare = _db.ClassSections.Where(x => x.ProgramId == f.ProgramId && x.Year == f.Year && x.Gender == f.Gender && x.Term == f.Term).FirstOrDefault();
                _db.ClassSections.Remove(compare);
                _db.ClassSections.Add(f);
            }
            foreach (var n in kpistatlist)
            {

                var compare = _db.Kpiprograms.Where(x => x.ProgramId == n.ProgramId && x.Year == n.Year && x.Term == n.Term && x.KpiId == n.KpiId && x.Gender == n.Gender).FirstOrDefault();
                _db.Kpiprograms.Remove(compare);
                _db.Kpiprograms.Add(n);
            }

            _db.SaveChanges();

            return View("Index");


        }

    }
}