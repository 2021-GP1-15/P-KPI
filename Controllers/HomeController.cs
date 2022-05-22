using IP_KPI.Dashboard;
using IP_KPI.Data;
using IP_KPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace IP_KPI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly db_a7baa5_ipkpiContext _db;
        List<Survey> data = new List<Survey>();
        String chape = "";
        String chape2 = "";
        public HomeController(ILogger<HomeController> logger, db_a7baa5_ipkpiContext db)
        {
            _logger = logger;
            _db = db;
        }

        [Authorize(Roles = "Decision Maker,Manager")]
        public IActionResult Index()
        {
            ViewBag.Colleges = _db.Colleges.ToList();
            ViewBag.KPI = _db.Kpis.ToList();
            return View();
        }

        [Authorize(Roles = "Manager,Decision Maker,Data Entry")]
        public IActionResult Homepage()
        {
            return View();
        }

        [Authorize(Roles = "Decision Maker,Manager")]
        public IActionResult tables()
        {
            ViewBag.Colleges = _db.Colleges.ToList();
            ViewBag.KPI = _db.Kpis.ToList();
            var KPIList = _db.Kpis.ToList();
            List<String> KPIListWithoutDublicatedName = new List<String>();
            for (var i = 0; i < KPIList.Count; i++)
            {
                var duplicated = false;
                for (var j = i - 1; j >= 0; j--)
                {
                    if (KPIList[i].Kpiname == KPIList[j].Kpiname)
                    {
                        duplicated = true;
                        break;
                    }
                }
                if (duplicated == false)
                {
                    KPIListWithoutDublicatedName.Add(KPIList[i].Kpiname);
                }

            }
            ViewBag.KPINames = KPIListWithoutDublicatedName;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        //Filter Code Start
        [HttpGet]
        public ActionResult GetDepartments(int collegeId)
        {
            try
            {
                var Departments = _db.Departments.Where(x => x.CollegeId == collegeId).Select(u => new { u.DepartmentName, u.DepartmentId }).ToList();
                return Json(Departments);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public ActionResult GetPrograms(int departmentId)
        {
            try
            {
                var Programs = _db.UniPrograms.Where(x => x.DepartmentId == departmentId).Select(u => new { u.ProgramName, u.ProgramId, u.Level }).ToList();
                return Json(Programs);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        // Filter code end


        // KPI-P-2,3 charts code start
        [HttpGet]
        public ActionResult GetSurveyChart(String programId, String KPICode, String gender, String term, String year, String studentCase, String nationality)
        {
            charts chart1 = new charts(_db);
            var kpi = _db.Kpis.FirstOrDefault(x => x.Kpicode == KPICode);
            String progName = chart1.progName(programId);

            //create a list for each chart
            var termList = _db.Surveys.Where(x => x.KpiId == kpi.KpiId && x.ProgramId == Convert.ToInt32(programId) && x.Year == year).ToList();
            var studentCaseList = _db.Surveys.Where(x => x.KpiId == kpi.KpiId && x.ProgramId == Convert.ToInt32(programId) && x.Year == year).ToList();
            var genderList = _db.Surveys.Where(x => x.KpiId == kpi.KpiId && x.ProgramId == Convert.ToInt32(programId) && x.Year == year).ToList();
            var nationalityList = _db.Surveys.Where(x => x.KpiId == kpi.KpiId && x.ProgramId == Convert.ToInt32(programId) && x.Year == year).ToList();
            var targetList = _db.Surveys.Where(x => x.KpiId == kpi.KpiId && x.ProgramId == Convert.ToInt32(programId) && x.Year == year).ToList();
            var KPIResult = _db.Kpiprograms.Where(x => x.KpiId == kpi.KpiId && x.ProgramId == Convert.ToInt32(programId) && x.Year == year).ToList();

            //filter the charts lists
            termList = chart1.filterList(termList, nationality, gender, studentCase, null);
            studentCaseList = chart1.filterList(studentCaseList, nationality, gender, null, term);
            genderList = chart1.filterList(genderList, nationality, null, studentCase, term);
            nationalityList = chart1.filterList(nationalityList, null, gender, studentCase, term);
            targetList = chart1.filterList(targetList, nationality, gender, studentCase, term);
            KPIResult = chart1.filterKPIList(KPIResult, term, gender);

            //declare the result and the bars color for each chart
            float firstTermScore = chart1.serveyScore(termList, 't', "الفصل الدراسي الاول"), secondTermScore = chart1.serveyScore(termList, 't', "الفصل الدراسي الثاني"), thirdTermScore = chart1.serveyScore(termList, 't', "الفصل الدراسي الثالث"), summerTermScore = chart1.serveyScore(termList, 't', "الفصل الدراسي الصيفي"), firstTermCount = chart1.serveyCount(termList, 't', "الفصل الدراسي الاول", null), secondTermCount = chart1.serveyCount(termList, 't', "الفصل الدراسي الثاني", null), thirdTermCount = chart1.serveyCount(termList, 't', "الفصل الدراسي الثالث", null), summerTermCount = chart1.serveyCount(termList, 't', "الفصل الدراسي الصيفي", null);
            String firstTermColor = "#D0DA32", secondTermColor = "#71BA44", thirdTermColor = "#29B8BE", summerTermColor = "#1F3771", bothTermColor = "#B0AEB3";//dark green
            float normalScore = chart1.serveyScore(studentCaseList, 's', "طبيعي"), specialScore = chart1.serveyScore(studentCaseList, 's', "احتياجات خاصة"), normalCount = chart1.serveyCount(studentCaseList, 's', "طبيعي", null), specialCount = chart1.serveyCount(studentCaseList, 's', "احتياجات خاصة", null);
            String normalColor = "#D0DA32", specialColor = "#1F3771";
            float fScore = chart1.serveyScore(genderList, 'g', "انثى"), mScore = chart1.serveyScore(genderList, 'g', "ذكر"), fCount = chart1.serveyCount(genderList, 'g', "انثى", null), mCount = chart1.serveyCount(genderList, 'g', "ذكر", null);
            String fColor = "#D0DA32", mColor = "#1F3771", bColor = "#B0AEB3";
            float inScore = chart1.serveyScore(nationalityList, 'n', "سعودي"), outScore = chart1.serveyScore(nationalityList, 'n', "غير سعودي"), inCount = chart1.serveyCount(nationalityList, 'n', "سعودي", null), outCount = chart1.serveyCount(nationalityList, 'n', "غير سعودي", null);
            String inColor = "#D0DA32", outColor = "#1F3771";
            var KPIScore = chart1.serveyScore(targetList, 'f', null) / chart1.serveyCount(targetList, 'f', null, null); float internalBenchmark = chart1.KPIscore(KPIResult, 'i'), externalBenchmark = chart1.KPIscore(KPIResult, 'e'), targetBenchmark = chart1.KPIscore(KPIResult, 'k'); var tenPercentOfTarget = targetBenchmark * .10;
            String KPIScoreColor;


            //############################################################
            //############################################################
            // color the chart bars based on the chosen filter options ###
            //############################################################
            //############################################################

            //###################### coloring start ######################
            if (term != "0")
            {
                if (term.Equals("الفصل الدراسي الاول"))
                {
                    firstTermColor = "#1F3771";//dark green
                    secondTermColor = "#B0AEB3";//light green
                    thirdTermColor = "#B0AEB3";//light green
                    summerTermColor = "#B0AEB3";//light green
                    bothTermColor = "#B0AEB3";//light green
                }
                else if (term.Equals("الفصل الدراسي الثاني"))
                {
                    secondTermColor = "#1F3771";//dark green
                    firstTermColor = "#B0AEB3";//light green
                    thirdTermColor = "#B0AEB3";//light green
                    summerTermColor = "#B0AEB3";//light green
                    bothTermColor = "#B0AEB3";//light green
                }
                else if (term.Equals("الفصل الدراسي الثالث"))
                {
                    thirdTermColor = "#1F3771";//dark green
                    firstTermColor = "#B0AEB3";//light green
                    secondTermColor = "#B0AEB3";//light green
                    summerTermColor = "#B0AEB3";//light green
                    bothTermColor = "#B0AEB3";//light green
                }
                else
                {
                    summerTermColor = "#1F3771";//dark green
                    firstTermColor = "#B0AEB3";//light green
                    secondTermColor = "#B0AEB3";//light green
                    thirdTermColor = "#B0AEB3";//light green
                    bothTermColor = "#B0AEB3";//light green
                }
            }
            if (studentCase != "0")
            {
                if (studentCase.Equals("طبيعي"))
                {
                    normalColor = "#1F3771";//dark yellow
                    specialColor = "#B0AEB3";//light grey
                }
                else
                {
                    specialColor = "#1F3771";//dark blue
                    normalColor = "#B0AEB3";//light grey
                }
            }
            if (gender != "0")
            {
                if (gender.Equals("انثى"))
                {
                    fColor = "#1F3771";//dark yellow
                    mColor = "#B0AEB3";//light grey
                    bColor = "#B0AEB3";//light grey
                }
                else
                {
                    mColor = "#1F3771";//dark blue
                    fColor = "#B0AEB3";//light grey
                    bColor = "#B0AEB3";//light grey
                }
            }
            if (nationality != "0")
            {
                if (nationality.Equals("سعودي"))
                {
                    inColor = "#1F3771";//dark yellow
                    outColor = "#B0AEB3";//light grey
                }
                else
                {
                    outColor = "#1F3771";//dark blue
                    inColor = "#B0AEB3";//light grey
                }
            }
            if (KPIScore <= (targetBenchmark - tenPercentOfTarget))
            {
                KPIScoreColor = "#C1272D";//red
                chape = "weave";
            }
            else if (KPIScore < targetBenchmark)
            {
                KPIScoreColor = "#F7931E";//yellow
                chape = "cross";
            }
            else if (KPIScore >= (targetBenchmark + tenPercentOfTarget))
            {
                KPIScoreColor = "#29B8BE";//blue
                chape = "dot";
            }
            else
            {
                KPIScoreColor = "#71BA44";//green
                chape = "line-vertical";
            }
            //###################### coloring end ########################


            //############################################################
            //############################################################
            //     send the data to the View to draw the charts        ###
            //############################################################
            //############################################################

            //################### term chart data start ##################
            data.Add(new Survey
            {
                Gender = "الفصل الاول",
                Nationality = firstTermColor,
                Term = "" + (firstTermScore / firstTermCount),
                ProgramId = (int)firstTermCount
            });
            data.Add(new Survey
            {
                Gender = "الفصل الثاني",
                Nationality = secondTermColor,
                Term = "" + (secondTermScore / secondTermCount),
                ProgramId = (int)secondTermCount
            });
            data.Add(new Survey
            {
                Gender = "الفصل الثالث",
                Nationality = thirdTermColor,
                Term = "" + (thirdTermScore / thirdTermCount),
                ProgramId = (int)thirdTermCount
            });
            data.Add(new Survey
            {
                Gender = "الفصل الصيفي",
                Nationality = summerTermColor,
                Term = "" + (summerTermScore / summerTermCount),
                ProgramId = (int)summerTermCount
            });
            data.Add(new Survey
            {
                Gender = "الكل",
                Nationality = bothTermColor,
                Term = "" + ((firstTermScore + secondTermScore + thirdTermScore + summerTermScore) / (firstTermCount + secondTermCount + thirdTermCount + summerTermCount)),
                ProgramId = (int)(firstTermCount + secondTermCount + thirdTermCount + summerTermCount)
            });
            //################### term chart data end ####################

            //################### student case chart data start ##########
            data.Add(new Survey
            {
                Gender = "طبيعي",
                Nationality = normalColor,
                Term = "" + (normalScore / normalCount),
                ProgramId = (int)normalCount
            });
            data.Add(new Survey
            {
                Gender = "احتياجات خاصة",
                Nationality = specialColor,
                Term = "" + (specialScore / specialCount),
                ProgramId = (int)specialCount
            });
            //################### student case chart data end ############

            //################### gender chart data start ################
            data.Add(new Survey
            {
                Gender = "انثى",
                Nationality = fColor,
                Term = "" + (fScore / fCount),
                ProgramId = (int)fCount
            });
            data.Add(new Survey
            {
                Gender = "ذكر",
                Nationality = mColor,
                Term = "" + (mScore / mCount),
                ProgramId = (int)mCount
            });
            data.Add(new Survey
            {
                Gender = "الكل",
                Nationality = bColor,
                Term = "" + ((fScore + mScore) / (fCount + mCount)),
                ProgramId = (int)(fCount + mCount)
            });
            //################### gender chart data end ##################

            //################### nationality chart data start ###########
            data.Add(new Survey
            {
                Gender = "سعودي",
                Nationality = inColor,
                Term = "" + (inScore / inCount),
                ProgramId = (int)inCount
            });
            data.Add(new Survey
            {
                Gender = "غير سعودي",
                Nationality = outColor,
                Term = "" + (outScore / outCount),
                ProgramId = (int)outCount
            });
            //################### nationality chart data start ###########

            //################### four value chart data start ############
            data.Add(new Survey
            {
                Gender = "القيمة الفعلية",
                Nationality = KPIScoreColor,
                Term = "" + KPIScore,
                StudentCase = progName,
                ProgramId = (int)chart1.serveyCount(targetList, 'f', null, null),
                Year = chape
            }); 
            data.Add(new Survey
            {
                Gender = "المقارنة المرجعية الخارجية",
                Nationality = "#D0DA32",
                Term = "" + externalBenchmark
            });
            data.Add(new Survey
            {
                Gender = "المقارنة المرجعية الداخلية",
                Nationality = "#B0AEB3",
                Term = "" + internalBenchmark
            });
            data.Add(new Survey
            {
                Gender = "الاداء المستهدف",
                Nationality = "#1F3771",
                Term = "" + targetBenchmark
            });
            //################### four value chart data end ##############

            return Json(data);

        }
        //KPI-P-2,3 charts code END

        //KPI-P-4,5 charts code start
        [HttpGet]
        public ActionResult GetStudentssChart(String programId, String KPICode, String gender, String year)
        {
            charts chart1 = new charts(_db);
            var kpi = _db.Kpis.FirstOrDefault(x => x.Kpicode == KPICode);
            String progName = chart1.progName(programId);

            var KPIResult = _db.Kpiprograms.Where(x => x.ProgramId == Convert.ToInt32(programId) && x.Year == year && x.KpiId == kpi.KpiId).ToList();
            KPIResult = chart1.filterKPIList(KPIResult, null, gender);
            var KPISCOR = _db.BatchStatistics.Where(x => x.ProgramId == Convert.ToInt32(programId) && x.Year == year).ToList();
            if (gender != "0")
            {
                KPISCOR = KPISCOR.Where(x => x.Gender == gender).ToList();
            }

            var numOfbatchS = 0.0; var numOfStudentsCont = 0.0; var MinYearsStudent = 0.0; var numFirstYearStudent = 0.0;
            foreach (var student in KPISCOR)
            {
                numOfbatchS += (int)student.NumOfBatchStudent;
                numOfStudentsCont += (int)student.NumStudentContinue;
                MinYearsStudent += (int)student.NumMinYearStudent;
                numFirstYearStudent += (int)student.NumFirstYearStudent;
            }




            String fColor = "#D0DA32", mColor = "#1F3771"; String KPIScoreColor;
            var KPIScore = 0.0; float internalBenchmark = chart1.KPIscore((List<Kpiprogram>)KPIResult, 'i'), externalBenchmark = chart1.KPIscore((List<Kpiprogram>)KPIResult, 'e'), targetBenchmark = chart1.KPIscore((List<Kpiprogram>)KPIResult, 'k'); var tenPercentOfTarget = targetBenchmark;
            if (KPICode.Equals("KPI-P-05"))
            {
                KPIScore = ((float)numOfStudentsCont / (float)numFirstYearStudent) *100;
            }
            if (KPICode.Equals("KPI-P-04"))
            {
                KPIScore = ((float)MinYearsStudent / (float)numOfbatchS) *100;
            }



            if (gender != "0")
            {
                if (gender.Equals("انثى"))
                {
                    fColor = "#D0DA32";//dark yellow
                    mColor = "#B0AEB3";//light grey
                }
                else
                {
                    mColor = "#1F3771";//dark blue
                    fColor = "#B0AEB3";//light grey
                }
            }

            //coloring the KPI result based on the target 
            if (KPIScore <= (targetBenchmark - tenPercentOfTarget))
            {
                KPIScoreColor = "#C1272D";//red
                chape = "weave";
            }
            else if (KPIScore < targetBenchmark)
            {
                KPIScoreColor = "#F7931E";//yellow
                chape = "cross";
            }
            else if (KPIScore >= (targetBenchmark + tenPercentOfTarget))
            {
                KPIScoreColor = "#29B8BE";//blue
                chape = "cross-dash";
            }
            else
            {
                KPIScoreColor = "#71BA44";//green
                chape = "line-vertical";
            }



            data.Add(new Survey
            {
                Gender = "القيمة الفعلية",
                Nationality = KPIScoreColor,
                Term = "" + KPIScore,
                StudentCase = progName, //program Name
                Year = chape
            });
            data.Add(new Survey
            {
                Gender = "المقارنة المرجعية الخارجية",
                Nationality = "#D0DA32",
                Term = "" + externalBenchmark
            });
            data.Add(new Survey
            {
                Gender = "المقارنة المرجعية الداخلية",
                Nationality = "#B0AEB3",
                Term = "" + internalBenchmark
            });
            data.Add(new Survey
            {
                Gender = "الاداء المستهدف",
                Nationality = "#1F3771",
                Term = "" + targetBenchmark
            });



            //pie1 for KPI-5
            data.Add(new Survey
            {
                Gender = "عدد الطلاب الذين اكملوا للسنة الثانية على نفس المسار",
                Nationality = fColor,
                Term = "" + (float)(numOfStudentsCont*100)/ (float)numFirstYearStudent,
                ProgramId = (int)(numOfStudentsCont)
            });
            data.Add(new Survey
            {
                Gender = "عدد الطلاب الذين لم يكملوا",
                Nationality = mColor,
                Term = "" + (float)((numFirstYearStudent - numOfStudentsCont)*100)/(float)numFirstYearStudent,
                ProgramId = (int)(numFirstYearStudent - numOfStudentsCont)
            });
            //end pie1

            //pie 2 for kpi-4
            data.Add(new Survey
            {
                Gender = "عدد الطلاب الذين تخرجوا في المدة المحددة",
                Nationality = fColor,
                Term = "" + (float)(MinYearsStudent*100)/ (float)numOfbatchS,
                ProgramId = (int)(MinYearsStudent)
            });


            data.Add(new Survey
            {
                Gender = "عدد الطلاب الذين لم يتخرجوا",
                Nationality = mColor,
                Term = "" + (float)((numOfbatchS - MinYearsStudent)*100)/(float)numOfbatchS,
                ProgramId = (int)(numOfbatchS - MinYearsStudent)
            });

            //end pie 2

            return Json(data);
        }
        //KPI-P-4,5 charts code END


        // KPI-P-6,7,9 charts code start
        [HttpGet]
        public ActionResult GetAlumniChart(String programId, String KPICode, String gender, String year)

        {
            charts chart1 = new charts(_db);
            var kpiStudent = _db.Kpis.FirstOrDefault(x => x.Kpicode == "KPI-P-02");
            var kpi = _db.Kpis.FirstOrDefault(x => x.Kpicode == KPICode);
            var kpi_06_B = _db.Kpis.FirstOrDefault(x => x.Kpicode == "KPI-P-06-B");
            String progName = chart1.progName(programId);

            //create a list for each chart
            var gradList = _db.Surveys.Where(x => x.KpiId == kpiStudent.KpiId && x.ProgramId == Convert.ToInt32(programId) && x.Year == year).ToList();
            gradList = chart1.filterList(gradList, null, gender, null, null);
            var gradCount = chart1.serveyCount(gradList, 'c', null, null); //total number of graduated students

            //participation In Extra Activity
            var numOfGradparticipated = gradList.Where(x => x.ExtraActivity == "مشترك").ToList();
            var numOfGradNotparticipated = gradList.Where(x => x.ExtraActivity == "غير مشترك").ToList();
            var gradParticipatedCount = chart1.serveyCount(numOfGradparticipated, 'c', null, null); //total number of graduated students participated
            var gradNotParticipatedCount = chart1.serveyCount(numOfGradNotparticipated, 'c', null, null); //total number of graduated students not participated

            var alumniList = _db.AlumniEmployments.Where(x => x.ProgramId == Convert.ToInt32(programId) && x.Year == year).ToList();
            var KPIResult = _db.Kpiprograms.Where(x => x.KpiId == kpi.KpiId && x.ProgramId == Convert.ToInt32(programId) && x.Year == year).ToList();
            var KPI_06_B_Result = _db.Kpiprograms.Where(x => x.KpiId == kpi_06_B.KpiId && x.ProgramId == Convert.ToInt32(programId) && x.Year == year).ToList();
            KPIResult = chart1.filterKPIList(KPIResult, null, gender);
            KPI_06_B_Result = chart1.filterKPIList(KPI_06_B_Result, null, gender);


            //declare the result and the bars color for each chart
            var KPIScore = 0.0; var KPIScore2 = 0.0;
            //KPI-7
            float countGradEmployedMale =0, countGradEmployedFemale = 0, countGradEmployed = 0, countGradEnrolledMale = 0, countGradEnrolledFemale = 0, countGradEnrolled = 0;
            if (KPICode == "KPI-P-07")
            {
                countGradEmployedMale = chart1.alumniResult(alumniList, "ذكر", 'e');
                countGradEmployedFemale = chart1.alumniResult(alumniList, "انثى", 'e');
                countGradEmployed = chart1.alumniResult(alumniList, gender, 'e');
                countGradEnrolledMale = chart1.alumniResult(alumniList, "ذكر", 'm');
                countGradEnrolledFemale = chart1.alumniResult(alumniList, "انثى", 'm');
                countGradEnrolled = chart1.alumniResult(alumniList, gender, 'm');
                KPIScore = ((countGradEmployed + countGradEnrolled) / gradCount) * 100;
            }
            //KPI-9
            float avgGradEvalByCoompanyMale = 0, avgGradEvalByCoompanyFemale = 0;
            if (KPICode == "KPI-P-09")
            {
                avgGradEvalByCoompanyMale = chart1.alumniResult(alumniList, "ذكر", 'c');
                avgGradEvalByCoompanyFemale = chart1.alumniResult(alumniList, "انثى", 'c');
                KPIScore = chart1.alumniResult(alumniList, gender, 'c');
            }
            //KPI-6
            float NumOfStudentPassQiyasMale = 0, NumOfStudentPassQiyasFemale = 0, NumOfStudentPassQiyas = 0, NumOfStudentPassCareerMale = 0, NumOfStudentPassCareerFemale = 0, NumOfStudentPassCareer = 0;
            if (KPICode == "KPI-P-06")
            {
                //KPI-6 part-1
                NumOfStudentPassQiyasMale = chart1.alumniResult(alumniList, "ذكر", 'q');
                NumOfStudentPassQiyasFemale = chart1.alumniResult(alumniList, "انثى", 'q');
                NumOfStudentPassQiyas = chart1.alumniResult(alumniList, gender, 'q');
                //KPI-6 part-2
                NumOfStudentPassCareerMale = chart1.alumniResult(alumniList, "ذكر", 'r');
                NumOfStudentPassCareerFemale = chart1.alumniResult(alumniList, "انثى", 'r');
                NumOfStudentPassCareer = chart1.alumniResult(alumniList, gender, 'r');

                KPIScore2 = (NumOfStudentPassCareer / gradCount) * 100;
                KPIScore = (NumOfStudentPassQiyas / gradCount) * 100;
            }

            float internalBenchmark = chart1.KPIscore(KPIResult, 'i'), externalBenchmark = chart1.KPIscore(KPIResult, 'e'), targetBenchmark = chart1.KPIscore(KPIResult, 'k'); var tenPercentOfTarget = targetBenchmark * .10;
            //for KPI_06_B only
            float internalBenchmark_B = chart1.KPIscore(KPI_06_B_Result, 'i'), externalBenchmark_B = chart1.KPIscore(KPI_06_B_Result, 'e'), targetBenchmark_B = chart1.KPIscore(KPI_06_B_Result, 'k'); var tenPercentOfTarget_B = targetBenchmark_B * .10;
            String KPIScoreColor, KPIScoreColor2;
            String fColor = "#D0DA32", mColor = "#1F3771";


            //############################################################
            //############################################################
            // color the chart bars based on the chosen filter options ###
            //############################################################
            //############################################################

            //###################### coloring start ######################
            if (KPIScore <= (targetBenchmark - tenPercentOfTarget))
            {
                KPIScoreColor = "#C1272D";//red
                chape = "weave";
            }
            else if (KPIScore < targetBenchmark)
            {
                KPIScoreColor = "#F7931E";//yellow
                chape = "cross";
            }
            else if (KPIScore >= (targetBenchmark + tenPercentOfTarget))
            {
                KPIScoreColor = "#29B8BE";//blue
                chape = "dot";
            }
            else
            {
                KPIScoreColor = "#71BA44";//green
                chape = "line-vertical";
            }



            //for KPI-06-B only
            if (KPIScore2 <= (targetBenchmark_B - tenPercentOfTarget_B))
            {
                KPIScoreColor2 = "#C1272D";//red
                chape2 = "weave";
            }
            else if (KPIScore2 < targetBenchmark_B)
            {
                KPIScoreColor2 = "#F7931E";//yellow
                chape2 = "cross";
            }
            else if (KPIScore2 >= (targetBenchmark_B + tenPercentOfTarget_B))
            {
                KPIScoreColor2 = "#29B8BE";//blue
                chape2 = "dot";
            }
            else
            {
                KPIScoreColor2 = "#71BA44";//green
                chape2 = "line-vertical";
            }



            if (gender != "0")
            {
                if (gender.Equals("انثى"))
                {
                    fColor = "#1F3771";//dark yellow
                    mColor = "#B0AEB3";//light grey
                }
                else
                {
                    mColor = "#1F3771";//dark blue
                    fColor = "#B0AEB3";//light grey
                }
            }
            //###################### coloring end ########################


            //############################################################
            //############################################################
            //     send the data to the View to draw the charts        ###
            //############################################################
            //############################################################

            //###### four value KPI-6(a),7,9 chart data start ############
            data.Add(new Survey //0
            {
                Gender = "القيمة الفعلية",
                Nationality = KPIScoreColor,
                Term = "" + KPIScore,
                StudentCase = progName,
                ProgramId = (int)gradCount,
                Year = chape
            });
            data.Add(new Survey //1
            {
                Gender = "المقارنة المرجعية الخارجية",
                Nationality = "#D0DA32",
                Term = "" + externalBenchmark
            });
            data.Add(new Survey //2
            {
                Gender = "المقارنة المرجعية الداخلية",
                Nationality = "#B0AEB3",
                Term = "" + internalBenchmark
            });
            data.Add(new Survey //3
            {
                Gender = "الاداء المستهدف",
                Nationality = "#1F3771",
                Term = "" + targetBenchmark
            });
            //###### four value KPI-6(a),7,9 chart data end ##############

            //###### gender pie KPI-7 employed chart data start ##########
            data.Add(new Survey //4
            {
                Gender = "انثى",
                Nationality = fColor,
                Term = "" + chart1.percentage(2, countGradEmployedFemale, countGradEmployedMale, 0, 0, 0, 0),
                ProgramId = (int)(countGradEmployedFemale),
                Year = ""+ countGradEmployed
            });
            data.Add(new Survey //5
            {
                Gender = "ذكر",
                Nationality = mColor,
                Term = "" + chart1.percentage(2, countGradEmployedMale, countGradEmployedFemale, 0, 0, 0, 0),
                ProgramId = (int)(countGradEmployedMale),
                Year = "dot"
            });
            //###### gender pie KPI-7 employed chart data end ############

            //###### gender pie KPI-7 continuMaster chart data start #####
            data.Add(new Survey //6
            {
                Gender = "انثى",
                Nationality = fColor,
                Term = "" + chart1.percentage(2, countGradEnrolledFemale, countGradEnrolledMale, 0, 0, 0, 0),
                ProgramId = (int)(countGradEnrolledFemale),
                Year = "" + countGradEnrolled
            });
            data.Add(new Survey //7
            {
                Gender = "ذكر",
                Nationality = mColor,
                Term = "" + chart1.percentage(2, countGradEnrolledMale, countGradEnrolledFemale, 0, 0, 0, 0),
                ProgramId = (int)(countGradEnrolledMale),
                Year = "dot"
            });
            //here 7
            //###### gender pie KPI-7 continuMaster chart data end #######

            //###### gender bar KPI-9 chart data start ###################
            data.Add(new Survey //8
            {
                Gender = "انثى",
                Nationality = fColor,
                Term = "" + avgGradEvalByCoompanyFemale
            });
            data.Add(new Survey //9
            {
                Gender = "ذكر",
                Nationality = mColor,
                Term = "" + avgGradEvalByCoompanyMale,
                Year = "dot"
            });
            //###### gender bar KPI-9 chart data end #####################

            //###### gender pie KPI-6(a) chart data start ################
            data.Add(new Survey //10
            {
                Gender = "انثى",
                Nationality = fColor,
                Term = "" + chart1.percentage(2, NumOfStudentPassQiyasFemale, NumOfStudentPassQiyasMale, 0, 0, 0, 0),
                ProgramId = (int)(NumOfStudentPassQiyasFemale),
                Year = "" + NumOfStudentPassQiyas
            });
            data.Add(new Survey //11
            {
                Gender = "ذكر",
                Nationality = mColor,
                Term = "" + chart1.percentage(2, NumOfStudentPassQiyasMale, NumOfStudentPassQiyasFemale, 0, 0, 0, 0),
                ProgramId = (int)(NumOfStudentPassQiyasMale),
                Year = "dot"
            });
            //###### gender pie KPI-6(a) chart data end ##################

            //###### gender pie KPI-6(b) chart data start ################
            data.Add(new Survey //12
            {
                Gender = "انثى",
                Nationality = fColor,
                Term = "" + chart1.percentage(2, NumOfStudentPassCareerFemale, NumOfStudentPassCareerMale, 0, 0, 0, 0),
                ProgramId = (int)(NumOfStudentPassCareerFemale),
                Year = "" + NumOfStudentPassCareer
            });
            data.Add(new Survey //13
            {
                Gender = "ذكر",
                Nationality = mColor,
                Term = "" + chart1.percentage(2, NumOfStudentPassCareerMale, NumOfStudentPassCareerFemale, 0, 0, 0, 0),
                ProgramId = (int)(NumOfStudentPassCareerMale),
                Year = "dot"
            });
            //###### gender pie KPI-6(b) chart data end ##################

            //###### four value KPI-6(b) chart data start ################
            data.Add(new Survey //14
            {
                Gender = "القيمة الفعلية",
                Nationality = KPIScoreColor2,
                Term = "" + KPIScore2,
                Year = chape2
            });
            data.Add(new Survey //15
            {
                Gender = "المقارنة المرجعية الخارجية",
                Nationality = "#D0DA32",
                Term = "" + externalBenchmark_B
            });
            data.Add(new Survey //16
            {
                Gender = "المقارنة المرجعية الداخلية",
                Nationality = "#B0AEB3",
                Term = "" + internalBenchmark_B
            });
            data.Add(new Survey //17
            {
                Gender = "الاداء المستهدف",
                Nationality = "#1F3771",
                Term = "" + targetBenchmark_B
            });
            //###### four value KPI-6(b) chart data end ################## 

            //#### Extra Activity participation pie KPI-7 data start #####
            data.Add(new Survey //18
            {
                Gender = "مشارك",
                Nationality = "#D0DA32",
                Term = "" + chart1.percentage(2, gradParticipatedCount, gradNotParticipatedCount, 0, 0, 0, 0),
                ProgramId = (int)(gradParticipatedCount)
            });
            data.Add(new Survey //19
            {
                Gender = "غير مشارك",
                Nationality = "#1F3771",
                Term = "" + chart1.percentage(2, gradNotParticipatedCount, gradParticipatedCount, 0, 0, 0, 0),
                ProgramId = (int)(gradNotParticipatedCount),
                Year = "dot"
            });
            //##### Extra Activity participation pie KPI-7 data end #####

            return Json(data);

        }
        // KPI-P-6,7,9 charts code end


        // KPI-P-8 charts code start
        [HttpGet]
        public ActionResult GetClassSectionChart(String programId, String KPICode, String gender, String term, String year)
        {
            charts chart1 = new charts(_db);
            var kpi = _db.Kpis.FirstOrDefault(x => x.Kpicode == KPICode);
            var kpiStudent = _db.Kpis.FirstOrDefault(x => x.Kpicode == "KPI-P-03");
            String progName = chart1.progName(programId);

            //create a list for each chart
            var termList = _db.ClassSections.Where(x => x.ProgramId == Convert.ToInt32(programId) && x.Year == year).ToList();
            var targetList = _db.ClassSections.Where(x => x.ProgramId == Convert.ToInt32(programId) && x.Year == year).ToList();
            var KPIResult = _db.Kpiprograms.Where(x => x.KpiId == kpi.KpiId && x.ProgramId == Convert.ToInt32(programId) && x.Year == year).ToList();
            var studentCount = _db.Surveys.Where(x => x.KpiId == kpiStudent.KpiId && x.ProgramId == Convert.ToInt32(programId) && x.Year == year).ToList();


            //filter the charts lists
            KPIResult = chart1.filterKPIList(KPIResult, term, gender);
            var studentCountOfKPI = chart1.filterList(studentCount, null, gender, null, term);


            //declare the result and the bars color for each chart
            var countOfStudent = chart1.serveyCount(studentCountOfKPI, 'c', null, null);
            var classes = chart1.numOfclasses(termList, gender, term);
            float firstTermFScore = chart1.serveyCount(studentCount, '2', "انثى", "الفصل الدراسي الاول") / chart1.numOfclasses(termList, "انثى", "الفصل الدراسي الاول"), secondTermFScore = chart1.serveyCount(studentCount, '2', "انثى", "الفصل الدراسي الثاني") / chart1.numOfclasses(termList, "انثى", "الفصل الدراسي الثاني"), thirdTermFScore = chart1.serveyCount(studentCount, '2', "انثى", "الفصل الدراسي الثالث") / chart1.numOfclasses(termList, "انثى", "الفصل الدراسي الثالث"), summerTermFScore = chart1.serveyCount(studentCount, '2', "انثى", "الفصل الدراسي الصيفي") / chart1.numOfclasses(termList, "انثى", "الفصل الدراسي الصيفي");
            float firstTermMScore = chart1.serveyCount(studentCount, '2', "ذكر", "الفصل الدراسي الاول") / chart1.numOfclasses(termList, "ذكر", "الفصل الدراسي الاول"), secondTermMScore = chart1.serveyCount(studentCount, '2', "ذكر", "الفصل الدراسي الثاني") / chart1.numOfclasses(termList, "ذكر", "الفصل الدراسي الثاني"), thirdTermMScore = chart1.serveyCount(studentCount, '2', "ذكر", "الفصل الدراسي الثالث") / chart1.numOfclasses(termList, "ذكر", "الفصل الدراسي الثالث"), summerTermMScore = chart1.serveyCount(studentCount, '2', "ذكر", "الفصل الدراسي الصيفي") / chart1.numOfclasses(termList, "ذكر", "الفصل الدراسي الصيفي");
            String firstTermColor = "#D0DA32", secondTermColor = "#D0DA32", thirdTermColor = "#D0DA32", summerTermColor = "#D0DA32", bothTermColor = "#D0DA32";//dark green
            String firstTermColorM = "#71BA44", secondTermColorM = "#71BA44", thirdTermColorM = "#71BA44", summerTermColorM = "#71BA44", bothTermColorM = "#71BA44";//dark green
            var KPIScore = countOfStudent / classes; float internalBenchmark = chart1.KPIscore(KPIResult, 'i'), externalBenchmark = chart1.KPIscore(KPIResult, 'e'), targetBenchmark = chart1.KPIscore(KPIResult, 'k'); var tenPercentOfTarget = targetBenchmark * .10;
            String KPIScoreColor;


            //############################################################
            //############################################################
            // color the chart bars based on the chosen filter options ###
            //############################################################
            //############################################################

            //###################### coloring start ######################
            if (term != "0")
            {
                if (gender != "0")
                {
                    if (gender.Equals("انثى"))
                    {
                        if (term.Equals("الفصل الدراسي الاول"))
                        {
                            firstTermColor = "#1F3771";//dark green
                            secondTermColor = "#B0AEB3";//light green
                            thirdTermColor = "#B0AEB3";//light green
                            summerTermColor = "#B0AEB3";//light green
                            bothTermColor = "#B0AEB3";//light green
                            firstTermColorM = "#B0AEB3";//dark green
                            secondTermColorM = "#B0AEB3";//light green
                            thirdTermColorM = "#B0AEB3";//light green
                            summerTermColorM = "#B0AEB3";//light green
                            bothTermColorM = "#B0AEB3";//light green
                        }
                        else if (term.Equals("الفصل الدراسي الثاني"))
                        {
                            secondTermColor = "#1F3771";//dark green
                            firstTermColor = "#B0AEB3";//light green
                            thirdTermColor = "#B0AEB3";//light green
                            summerTermColor = "#B0AEB3";//light green
                            bothTermColor = "#B0AEB3";//light green
                            firstTermColorM = "#B0AEB3";//dark green
                            secondTermColorM = "#B0AEB3";//light green
                            thirdTermColorM = "#B0AEB3";//light green
                            summerTermColorM = "#B0AEB3";//light green
                            bothTermColorM = "#B0AEB3";//light green
                        }
                        else if (term.Equals("الفصل الدراسي الثالث"))
                        {
                            thirdTermColor = "#1F3771";//dark green
                            firstTermColor = "#B0AEB3";//light green
                            secondTermColor = "#B0AEB3";//light green
                            summerTermColor = "#B0AEB3";//light green
                            bothTermColor = "#B0AEB3";//light green
                            firstTermColor = "#B0AEB3";//light green
                            secondTermColor = "#B0AEB3";//light green
                            summerTermColor = "#B0AEB3";//light green
                            bothTermColor = "#B0AEB3";//light green
                            firstTermColorM = "#B0AEB3";//dark green
                            secondTermColorM = "#B0AEB3";//light green
                            thirdTermColorM = "#B0AEB3";//light green
                            summerTermColorM = "#B0AEB3";//light green
                            bothTermColorM = "#B0AEB3";//light green
                        }
                        else
                        {
                            summerTermColor = "#1F3771";//dark green
                            firstTermColor = "#B0AEB3";//light green
                            secondTermColor = "#B0AEB3";//light green
                            thirdTermColor = "#B0AEB3";//light green
                            bothTermColor = "#B0AEB3";//light green
                            firstTermColorM = "#B0AEB3";//dark green
                            secondTermColorM = "#B0AEB3";//light green
                            thirdTermColorM = "#B0AEB3";//light green
                            summerTermColorM = "#B0AEB3";//light green
                            bothTermColorM = "#B0AEB3";//light green
                        }
                    }
                    else
                    {
                        if (term.Equals("الفصل الدراسي الاول"))
                        {
                            firstTermColor = "#B0AEB3";//dark green
                            secondTermColor = "#B0AEB3";//light green
                            thirdTermColor = "#B0AEB3";//light green
                            summerTermColor = "#B0AEB3";//light green
                            bothTermColor = "#B0AEB3";//light green
                            firstTermColorM = "#1F3771";//dark green
                            secondTermColorM = "#B0AEB3";//light green
                            thirdTermColorM = "#B0AEB3";//light green
                            summerTermColorM = "#B0AEB3";//light green
                            bothTermColorM = "#B0AEB3";//light green
                        }
                        else if (term.Equals("الفصل الدراسي الثاني"))
                        {
                            secondTermColor = "#B0AEB3";//dark green
                            firstTermColor = "#B0AEB3";//light green
                            thirdTermColor = "#B0AEB3";//light green
                            summerTermColor = "#B0AEB3";//light green
                            bothTermColor = "#B0AEB3";//light green
                            firstTermColorM = "#B0AEB3";//dark green
                            secondTermColorM = "#1F3771";//light green
                            thirdTermColorM = "#B0AEB3";//light green
                            summerTermColorM = "#B0AEB3";//light green
                            bothTermColorM = "#B0AEB3";//light green
                        }
                        else if (term.Equals("الفصل الدراسي الثالث"))
                        {
                            thirdTermColor = "#B0AEB3";//dark green
                            firstTermColor = "#B0AEB3";//light green
                            secondTermColor = "#B0AEB3";//light green
                            summerTermColor = "#B0AEB3";//light green
                            bothTermColor = "#B0AEB3";//light green
                            firstTermColor = "#B0AEB3";//light green
                            secondTermColor = "#B0AEB3";//light green
                            summerTermColor = "#B0AEB3";//light green
                            bothTermColor = "#B0AEB3";//light green
                            firstTermColorM = "#B0AEB3";//dark green
                            secondTermColorM = "#B0AEB3";//light green
                            thirdTermColorM = "#1F3771";//light green
                            summerTermColorM = "#B0AEB3";//light green
                            bothTermColorM = "#B0AEB3";//light green

                        }
                        else
                        {
                            summerTermColor = "#B0AEB3";//dark green
                            firstTermColor = "#B0AEB3";//light green
                            secondTermColor = "#B0AEB3";//light green
                            thirdTermColor = "#B0AEB3";//light green
                            bothTermColor = "#B0AEB3";//light green
                            firstTermColorM = "#B0AEB3";//dark green
                            secondTermColorM = "#B0AEB3";//light green
                            thirdTermColorM = "#B0AEB3";//light green
                            summerTermColorM = "#1F3771";//light green
                            bothTermColorM = "#B0AEB3";//light green
                        }
                    }
                }
                else
                {
                    if (term.Equals("الفصل الدراسي الاول"))
                    {
                        firstTermColor = "#1F3771";//dark green
                        secondTermColor = "#B0AEB3";//light green
                        thirdTermColor = "#B0AEB3";//light green
                        summerTermColor = "#B0AEB3";//light green
                        bothTermColor = "#B0AEB3";//light green
                        firstTermColorM = "#1F3771";//dark green
                        secondTermColorM = "#B0AEB3";//light green
                        thirdTermColorM = "#B0AEB3";//light green
                        summerTermColorM = "#B0AEB3";//light green
                        bothTermColorM = "#B0AEB3";//light green
                    }
                    else if (term.Equals("الفصل الدراسي الثاني"))
                    {
                        secondTermColor = "#1F3771";//dark green
                        firstTermColor = "#B0AEB3";//light green
                        thirdTermColor = "#B0AEB3";//light green
                        summerTermColor = "#B0AEB3";//light green
                        bothTermColor = "#B0AEB3";//light green
                        firstTermColorM = "#B0AEB3";//dark green
                        secondTermColorM = "#1F3771";//light green
                        thirdTermColorM = "#B0AEB3";//light green
                        summerTermColorM = "#B0AEB3";//light green
                        bothTermColorM = "#B0AEB3";//light green
                    }
                    else if (term.Equals("الفصل الدراسي الثالث"))
                    {
                        thirdTermColor = "#1F3771";//dark green
                        firstTermColor = "#B0AEB3";//light green
                        secondTermColor = "#B0AEB3";//light green
                        summerTermColor = "#B0AEB3";//light green
                        bothTermColor = "#B0AEB3";//light green
                        firstTermColorM = "#B0AEB3";//dark green
                        secondTermColorM = "#B0AEB3";//light green
                        thirdTermColorM = "#1F3771";//light green
                        summerTermColorM = "#B0AEB3";//light green
                        bothTermColorM = "#B0AEB3";//light green

                    }
                    else
                    {
                        summerTermColor = "#1F3771";//dark green
                        firstTermColor = "#B0AEB3";//light green
                        secondTermColor = "#B0AEB3";//light green
                        thirdTermColor = "#B0AEB3";//light green
                        bothTermColor = "#B0AEB3";//light green
                        firstTermColorM = "#B0AEB3";//dark green
                        secondTermColorM = "#B0AEB3";//light green
                        thirdTermColorM = "#B0AEB3";//light green
                        summerTermColorM = "#1F3771";//light green
                        bothTermColorM = "#B0AEB3";//light green

                    }
                }
            }
            else
            {
                if (gender != "0")
                {
                    if (gender.Equals("انثى"))
                    {
                            firstTermColor = "#1F3771";//dark green
                            secondTermColor = "#1F3771";//light green
                            thirdTermColor = "#1F3771";//light green
                            summerTermColor = "#1F3771";//light green
                            bothTermColor = "#1F3771";//light green
                            firstTermColorM = "#B0AEB3";//dark green
                            secondTermColorM = "#B0AEB3";//light green
                            thirdTermColorM = "#B0AEB3";//light green
                            summerTermColorM = "#B0AEB3";//light green
                            bothTermColorM = "#B0AEB3";//light green
                    }
                    else
                    {
                            firstTermColor = "#B0AEB3";//dark green
                            secondTermColor = "#B0AEB3";//light green
                            thirdTermColor = "#B0AEB3";//light green
                            summerTermColor = "#B0AEB3";//light green
                            bothTermColor = "#B0AEB3";//light green
                            firstTermColorM = "#1F3771";//dark green
                            secondTermColorM = "#1F3771";//light green
                            thirdTermColorM = "#1F3771";//light green
                            summerTermColorM = "#1F3771";//light green
                            bothTermColorM = "#1F3771";//light green
                    }
                }
            }

            if (KPIScore <= (targetBenchmark - tenPercentOfTarget))
            {
                KPIScoreColor = "#C1272D";//red
                chape = "weave";
            }
            else if (KPIScore < targetBenchmark)
            {
                KPIScoreColor = "#F7931E";//yellow
                chape = "cross";
            }
            else if (KPIScore >= (targetBenchmark + tenPercentOfTarget))
            {
                KPIScoreColor = "#29B8BE";//blue
                chape = "dot";
            }
            else
            {
                KPIScoreColor = "#71BA44";//green
                chape = "line-vertical";
            }
            //###################### coloring end ########################


            //############################################################
            //############################################################
            //     send the data to the View to draw the charts        ###
            //############################################################
            //############################################################

            //################### four value chart data start ############
            data.Add(new Survey //0
            {
                Gender = "القيمة الفعلية",
                Nationality = KPIScoreColor,
                Term = "" + KPIScore,
                StudentCase = chape,
                ProgramId = (int)countOfStudent,
                Year = "" + classes
            });
            data.Add(new Survey //1
            {
                Gender = "المقارنة المرجعية الخارجية",
                Nationality = "#D0DA32",
                Term = "" + externalBenchmark,
                StudentCase = progName
            });
            data.Add(new Survey //2
            {
                Gender = "المقارنة المرجعية الداخلية",
                Nationality = "#B0AEB3",
                Term = "" + internalBenchmark
            });
            data.Add(new Survey //3
            {
                Gender = "الاداء المستهدف",
                Nationality = "#1F3771",
                Term = "" + targetBenchmark
            });
            //################### four value chart data end ##############

            //################### term chart data start ##################
            data.Add(new Survey //4
            {
                Gender = "الفصل الدراسي الاول",
                Nationality = firstTermColor,
                Term = "" + firstTermFScore
            });
            data.Add(new Survey //5
            {
                Gender = "الفصل الدراسي الثاني",
                Nationality = secondTermColor,
                Term = "" + secondTermFScore
            });
            data.Add(new Survey //6
            {
                Gender = "الفصل الدراسي الثالث",
                Nationality = thirdTermColor,
                Term = "" + thirdTermFScore
            });
            data.Add(new Survey //7
            {
                Gender = "الفصل الدراسي الصيفي",
                Nationality = summerTermColor,
                Term = "" + summerTermFScore
            });
            data.Add(new Survey //8
            {
                Gender = "الكل",
                Nationality = bothTermColor,
                Term = "" + (firstTermFScore + secondTermFScore + thirdTermFScore + summerTermFScore)
            });
            data.Add(new Survey //9
            {
                Gender = "الفصل الدراسي الاول",
                Nationality = firstTermColorM,
                Term = "" + firstTermMScore
            });
            data.Add(new Survey //10
            {
                Gender = "الفصل الدراسي الثاني",
                Nationality = secondTermColorM,
                Term = "" + secondTermMScore
            });
            data.Add(new Survey //11
            {
                Gender = "الفصل الدراسي الثالث",
                Nationality = thirdTermColorM,
                Term = "" + thirdTermMScore
            });
            data.Add(new Survey //12
            {
                Gender = "الفصل الدراسي الصيفي",
                Nationality = summerTermColorM,
                Term = "" + summerTermMScore
            });
            data.Add(new Survey //13
            {
                Gender = "الكل",
                Nationality = bothTermColorM,
                Term = "" + (firstTermMScore + secondTermMScore + thirdTermMScore + summerTermMScore)
            });
            //################### term chart data end ####################

            return Json(data);

        }
        //KPI-P-8 charts code END

        // KPI-P-10,17 charts code start
        public ActionResult GetResorcesChart(String programId, String KPICode, String gender, String year, String studentCase, String nationality)
        {
            var kpi = _db.Kpis.FirstOrDefault(x => x.Kpicode == KPICode);
            charts chart1 = new charts(_db);
            String progName = chart1.progName(programId);

            var NumOfMR = _db.Surveys.Where(x => x.KpiId == kpi.KpiId && x.Year == year && x.ProgramId == Convert.ToInt32(programId)).ToList();
            var NumOfFR = _db.Surveys.Where(x => x.KpiId == kpi.KpiId && x.Year == year && x.ProgramId == Convert.ToInt32(programId)).ToList();

            NumOfMR = chart1.filterList(NumOfMR, nationality, "ذكر", studentCase, null);
            NumOfFR = chart1.filterList(NumOfFR, nationality, "انثى", studentCase, null);

            int goodScoreMR = 0, ExcelantScoreMR = 0, NeutralScoreMR = 0, weakScoreMR = 0, sumOfSurveyResorcesMR = 0, numStudentMR = 0;
            foreach (var numOfMR in NumOfMR)
            {
                goodScoreMR += (int)numOfMR.NumOfGoodScore;

                ExcelantScoreMR += (int)numOfMR.NumOfExcellentScore;

                NeutralScoreMR += (int)numOfMR.NumOfNeutralScore;

                weakScoreMR += (int)numOfMR.NumOfWeakScore;

                sumOfSurveyResorcesMR += (int)numOfMR.SurveyScore;

                numStudentMR += (int)numOfMR.NumOfRespondent;

            }
            int goodScoreFR = 0, ExcelantScoreFR = 0, NeutralScoreFR = 0, weakScoreFR = 0, sumOfSurveyResorcesFR = 0, numStudentFR = 0;
            foreach (var numOfFR in NumOfFR)
            {
                goodScoreFR += (int)numOfFR.NumOfGoodScore;

                ExcelantScoreFR += (int)numOfFR.NumOfExcellentScore;

                NeutralScoreFR += (int)numOfFR.NumOfNeutralScore;

                weakScoreFR += (int)numOfFR.NumOfWeakScore;


                sumOfSurveyResorcesFR += (int)numOfFR.SurveyScore;

                numStudentFR += (int)numOfFR.NumOfRespondent;
            }

            var FacultyF = _db.FacultyStatistics.Where(x => x.Year == year && x.Gender == "انثى" && x.ProgramId == Convert.ToInt32(programId)).ToList();
            var numOfFacultyF = 0; var goodScoreFRFAC = 0; var ExcelantScoreFRFAC = 0; var NeutralScoreFRFAC = 0; var weakScoreFRFAC = 0; var sumOfSurveyResorcesFRFAC = 0;

            foreach (var facultyF in FacultyF)
            {
                numOfFacultyF += (int)facultyF.NumOfRespondent;
                goodScoreFRFAC += (int)facultyF.NumOfGoodScore;
                ExcelantScoreFRFAC += (int)facultyF.NumOfExellentScore;
                NeutralScoreFRFAC += (int)facultyF.NumOfNeutralScore;
                weakScoreFRFAC += (int)facultyF.NumOfWeakScore;
                sumOfSurveyResorcesFRFAC += (int)facultyF.SurveyScore;
            }
            var FacultyM = _db.FacultyStatistics.Where(x => x.Year == year && x.Gender == "ذكر" && x.ProgramId == Convert.ToInt32(programId)).ToList();
            var numOfFacultyM = 0; var goodScoreMRFAC = 0; var ExcelantScoreMRFAC = 0; var NeutralScoreMRFAC = 0; var weakScoreMRFAC = 0; var sumOfSurveyResorcesMRFAC = 0;
            foreach (var facultyM in FacultyM)
            {
                numOfFacultyM += (int)facultyM.NumOfRespondent;
                goodScoreMRFAC += (int)facultyM.NumOfGoodScore;
                ExcelantScoreMRFAC += (int)facultyM.NumOfExellentScore;
                NeutralScoreMRFAC += (int)facultyM.NumOfNeutralScore;
                weakScoreMRFAC += (int)facultyM.NumOfWeakScore;
                sumOfSurveyResorcesMRFAC += (int)facultyM.SurveyScore;
            }


            var KPIResult = _db.Kpiprograms.Where(x => x.ProgramId == Convert.ToInt32(programId) && x.Year == year && x.KpiId == kpi.KpiId).ToList();
            String fColor = "#D0DA32", mColor = "#1F3771"; String KPIScoreColor;
            
            //### Scoring the KPI values ###//
            var KPIScore = 0.0;  float internalBenchmark = chart1.KPIscore(KPIResult, 'i'), externalBenchmark = chart1.KPIscore(KPIResult, 'e'), targetBenchmark = chart1.KPIscore(KPIResult, 'k'); var tenPercentOfTarget = targetBenchmark * .10;
            if (KPICode.Equals("KPI-P-10") && gender == "0")
                KPIScore = (double)(sumOfSurveyResorcesFR + sumOfSurveyResorcesMR) / (double)(numStudentFR + numStudentMR);

            else if (KPICode.Equals("KPI-P-10") && gender == "انثى")
                KPIScore = (double)sumOfSurveyResorcesFR / (double)numStudentFR;

            else
                KPIScore = (double)sumOfSurveyResorcesMR / (double)numStudentMR;

            if (KPICode.Equals("KPI-P-17") && gender == "0")
                KPIScore = (double)(sumOfSurveyResorcesFR + sumOfSurveyResorcesMR + sumOfSurveyResorcesFRFAC + sumOfSurveyResorcesMRFAC) / (double)(numStudentFR + numStudentMR + numOfFacultyF + numOfFacultyF);

            else if (KPICode.Equals("KPI-P-17") && gender == "انثى")
                KPIScore = (double)(sumOfSurveyResorcesFR + sumOfSurveyResorcesFRFAC) / (double)(numStudentFR + numOfFacultyF);
            
            else
                KPIScore = (double)(sumOfSurveyResorcesMR + sumOfSurveyResorcesMRFAC) / (double)(numStudentMR + numOfFacultyM);



            if (gender != "0")
            {
                if (gender.Equals("انثى"))
                {
                    fColor = "#1F3771";//dark yellow
                    mColor = "#B0AEB3";//light grey

                }
                else
                {
                    mColor = "#1F3771";//dark blue
                    fColor = "#B0AEB3";//light grey
                }
            }

            if (KPIScore <= (targetBenchmark - tenPercentOfTarget))
            {
                KPIScoreColor = "#C1272D";//red
                chape = "weave";
            }
            else if (KPIScore < targetBenchmark)
            {
                KPIScoreColor = "#F7931E";//yellow
                chape = "cross";
            }
            else if (KPIScore >= (targetBenchmark + tenPercentOfTarget))
            {
                KPIScoreColor = "#29B8BE";//blue
                chape = "dot";
            }
            else
            {
                KPIScoreColor = "#71BA44";//green
                chape = "line-vertical";
            }

            data.Add(new Survey //0
            {
                Gender = "القيمة الفعلية",
                Nationality = KPIScoreColor,
                Term = "" + KPIScore,
                StudentCase = progName, //program Name
                Year = chape
            });

            data.Add(new Survey //1
            {
                Gender = "المقارنة المرجعية الخارجية",
                Nationality = "#D0DA32",
                Term = "" + externalBenchmark,
                StudentCase = progName
            });
            data.Add(new Survey //2
            {
                Gender = "المقارنة المرجعية الداخلية",
                Nationality = "#B0AEB3",
                Term = "" + internalBenchmark,
                StudentCase = progName
            });
            data.Add(new Survey //3
            {
                Gender = "الاداء المستهدف",
                Nationality = "#1F3771",
                Term = "" + targetBenchmark,
                StudentCase = progName
            });


            if (gender == "انثى")
            {
                data.Add(new Survey //5 -1
                {
                    Gender = "عدد المقيممين بممتاز",
                    Nationality = "#D0DA32",
                    Term = "" + ExcelantScoreFR,
                    StudentCase = progName
                });

                data.Add(new Survey //6
                {
                    Gender = "عدد المقيممين بجيدجدا",
                    Nationality = "#71BA44",
                    Term = "" + goodScoreFR,
                    StudentCase = progName
                });

                data.Add(new Survey //7
                {
                    Gender = "عدد المقيممين بجيد",
                    Nationality = "#29B8BE",
                    Term = "" + NeutralScoreFR,
                    StudentCase = progName
                });
                data.Add(new Survey //8
                {
                    Gender = "عدد المقيممين بمقبول",
                    Nationality = "#1F3771",
                    Term = "" + weakScoreFR,
                    StudentCase = progName
                });

                data.Add(new Survey //9
                {
                    Gender = "عدد المقيممين بممتاز",
                    Nationality = "#D0DA32",
                    Term = "" + ExcelantScoreFRFAC,
                    StudentCase = progName
                });

                data.Add(new Survey //10
                {
                    Gender = "عدد المقيممين بجيدجدا",
                    Nationality = "#71BA44",
                    Term = "" + goodScoreFRFAC,
                    StudentCase = progName
                });

                data.Add(new Survey //11
                {
                    Gender = "عدد المقيممين بجيد",
                    Nationality = "#29B8BE",
                    Term = "" + NeutralScoreFRFAC,
                    StudentCase = progName
                });
                data.Add(new Survey //12
                {
                    Gender = "عدد المقيممين بمقبول",
                    Nationality = "#1F3771",
                    Term = "" + weakScoreFRFAC,
                    StudentCase = progName
                });


            }
            else if (gender == "ذكر")
            {
                data.Add(new Survey //5
                {
                    Gender = "عدد المقيممين بممتاز",
                    Nationality = "#D0DA32",
                    Term = "" + ExcelantScoreMR,
                    StudentCase = progName
                });

                data.Add(new Survey //6
                {
                    Gender = "عدد المقيممين بجيدجدا",
                    Nationality = "#71BA44",
                    Term = "" + goodScoreMR,
                    StudentCase = progName
                });

                data.Add(new Survey //7
                {
                    Gender = "عدد المقيممين بجيد",
                    Nationality = "#29B8BE",
                    Term = "" + NeutralScoreMR,
                    StudentCase = progName
                });
                data.Add(new Survey //8
                {
                    Gender = "عدد المقيممين بمقبول",
                    Nationality = "#1F3771",
                    Term = "" + weakScoreMR,
                    StudentCase = progName
                });

                data.Add(new Survey //9
                {
                    Gender = "عدد المقيممين بجيدجدا",
                    Nationality = "#71BA44",
                    Term = "" + goodScoreMRFAC,
                    StudentCase = progName
                });

                data.Add(new Survey //10
                {
                    Gender = "عدد المقيممين بجيد",
                    Nationality = "#29B8BE",
                    Term = "" + NeutralScoreMRFAC,
                    StudentCase = progName
                });
                data.Add(new Survey //11
                {
                    Gender = "عدد المقيممين بمقبول",
                    Nationality = "#1F3771",
                    Term = "" + weakScoreMRFAC,
                    StudentCase = progName
                });

                data.Add(new Survey //12
                {
                    Gender = "عدد المقيممين بممتاز",
                    Nationality = "#D0DA32",
                    Term = "" + (double)ExcelantScoreMRFAC,
                    StudentCase = progName
                });

            }
            else
            {
                data.Add(new Survey //5
                {
                    Gender = "عدد المقيممين بممتاز",
                    Nationality = "#D0DA32",
                    Term = "" + (double)(ExcelantScoreFR + ExcelantScoreMR),
                    StudentCase = progName
                });

                data.Add(new Survey //6
                {
                    Gender = "عدد المقيممين بجيدجدا",
                    Nationality = "#71BA44",
                    Term = "" + (double)(goodScoreFR + goodScoreMR),
                    StudentCase = progName
                });

                data.Add(new Survey //7
                {
                    Gender = "عدد المقيممين بجيد",
                    Nationality = "#29B8BE",
                    Term = "" + (double)(NeutralScoreFR + NeutralScoreMR),
                    StudentCase = progName
                });
                data.Add(new Survey //8
                {
                    Gender = "عدد المقيممين بمقبول",
                    Nationality = "#1F3771",
                    Term = "" + (double)(weakScoreFR + weakScoreMR),
                    StudentCase = progName
                });
                data.Add(new Survey //9
                {
                    Gender = "عدد المقيممين بجيدجدا",
                    Nationality = "#71BA44",
                    Term = "" + (goodScoreMRFAC + goodScoreFRFAC),
                    StudentCase = progName
                });

                data.Add(new Survey //10
                {
                    Gender = "عدد المقيممين بجيد",
                    Nationality = "#29B8BE",
                    Term = "" + (NeutralScoreMRFAC + NeutralScoreFRFAC),
                    StudentCase = progName
                });
                data.Add(new Survey //11
                {
                    Gender = "عدد المقيممين بمقبول",
                    Nationality = "#1F3771",
                    Term = "" + (weakScoreMRFAC + weakScoreFRFAC),
                    StudentCase = progName
                });

                data.Add(new Survey //12
                {
                    Gender = "عدد المقيممين بممتاز",
                    Nationality = "#D0DA32",
                    Term = "" + (double)(ExcelantScoreMRFAC + ExcelantScoreFRFAC),
                    StudentCase = progName
                });
            }

            data.Add(new Survey //13
            {
                Gender = "عدد الطلاب المشاركين في الاستبيان",
                Nationality = mColor,
                Term = "" + (sumOfSurveyResorcesMR/ numStudentMR),
                ProgramId= numStudentMR,
                StudentCase = progName
            });
            data.Add(new Survey //14
            {
                Gender = "عدد الطالبات المشاركات في الاستبيان",
                Nationality = fColor,
                Term = "" + (sumOfSurveyResorcesFR / numStudentFR),
                ProgramId = numStudentFR,
                StudentCase = progName
            });

            data.Add(new Survey //15
            {
                Gender = "عدد الاعضاء المشاركين في الاستبيان",
                Nationality = mColor,
                Term = "" + (sumOfSurveyResorcesMRFAC / numOfFacultyM),
                ProgramId = numOfFacultyM,
                StudentCase = progName
            });
            data.Add(new Survey //16
            {
                Gender = "عدد العضوات المشاركات في الاستبيان",
                Nationality = fColor,
                Term = "" + (sumOfSurveyResorcesFRFAC / numOfFacultyF),
                ProgramId = numOfFacultyF,
                StudentCase = progName
            });

            return Json(data);
        }

        // KPI-P-10,17 charts code end


        // KPI-P-11-13 charts code start
        [HttpGet]
        public ActionResult GetFacultyStatisticsChart(String programId, String KPICode, String gender, String year)
        {
            charts chart1 = new charts(_db);
            var kpiStudent = _db.Kpis.FirstOrDefault(x => x.Kpicode == "KPI-P-03");
            var kpi = _db.Kpis.FirstOrDefault(x => x.Kpicode == KPICode);
            String progName = chart1.progName(programId);

            //create a list for each chart
            var studentCount = _db.Surveys.Where(x => x.KpiId == kpiStudent.KpiId && x.ProgramId == Convert.ToInt32(programId) && x.Year == year).ToList();
            studentCount = chart1.filterList(studentCount, null, gender, null, null);
            var facultyList = _db.FacultyStatistics.Where(x => x.ProgramId == Convert.ToInt32(programId) && x.Year == year).ToList();

            var KPIResult = _db.Kpiprograms.Where(x => x.KpiId == kpi.KpiId && x.ProgramId == Convert.ToInt32(programId) && x.Year == year).ToList();
            KPIResult = chart1.filterKPIList(KPIResult, null, gender);

            //declare the result and the bars color for each chart
            var countOfStudent = chart1.serveyCount(studentCount, 'c', null, null);
            var countKPI_11 = chart1.numOfFaculty(facultyList, gender, "على راس العمل", 'a');

            var countAllFaculty = chart1.numOfFaculty(facultyList, gender, null, 'a');
            var countKPI_12_Male = chart1.numOfFaculty(facultyList, "ذكر", null, 'a');
            var countKPI_12_Female = chart1.numOfFaculty(facultyList, "انثى", null, 'a');

            var countKPI_12_FemaleAssistantPorf = chart1.numOfFaculty(facultyList, "انثى", null, 'b');
            var countKPI_12_FemaleAssociateProf = chart1.numOfFaculty(facultyList, "انثى", null, 'c');
            var countKPI_12_FemaleLecturer = chart1.numOfFaculty(facultyList, "انثى", null, 'd');
            var countKPI_12_FemaleProf = chart1.numOfFaculty(facultyList, "انثى", null, 'e');
            var countKPI_12_FemaleTeacher = chart1.numOfFaculty(facultyList, "انثى", null, 'f');
            var countKPI_12_FemaleTeachingAssistant = chart1.numOfFaculty(facultyList, "انثى", null, 'g');
            var countKPI_12_MaleAssistantPorf = chart1.numOfFaculty(facultyList, "ذكر", null, 'b');
            var countKPI_12_MaleAssociateProf = chart1.numOfFaculty(facultyList, "ذكر", null, 'c');
            var countKPI_12_MaleLecturer = chart1.numOfFaculty(facultyList, "ذكر", null, 'd');
            var countKPI_12_MaleProf = chart1.numOfFaculty(facultyList, "ذكر", null, 'e');
            var countKPI_12_MaleTeacher = chart1.numOfFaculty(facultyList, "ذكر", null, 'f');
            var countKPI_12_MaleTeachingAssistant = chart1.numOfFaculty(facultyList, "ذكر", null, 'g');

            var countKPI_13 = chart1.numOfFaculty(facultyList, gender, null, 'r');
            var countKPI_13_MaleResignation = chart1.numOfFaculty(facultyList, "ذكر", null, 'r');
            var countKPI_13_FemaleResignation = chart1.numOfFaculty(facultyList, "انثى", null, 'r');

            var KPIScore = 0.0;
            if (KPICode == "KPI-P-11")
                KPIScore = countOfStudent / countKPI_11;
            else if (KPICode == "KPI-P-13")
                KPIScore = (countKPI_13 / countAllFaculty) * 100;
            else if(KPICode == "KPI-P-12")
                KPIScore = (countKPI_11 / countAllFaculty) * 100;

            float internalBenchmark = chart1.KPIscore(KPIResult, 'i'), externalBenchmark = chart1.KPIscore(KPIResult, 'e'), targetBenchmark = chart1.KPIscore(KPIResult, 'k'); var tenPercentOfTarget = targetBenchmark * .10;
            String KPIScoreColor;
            String fColor = "#D0DA32", mColor = "#1F3771";


            //############################################################
            //############################################################
            // color the chart bars based on the chosen filter options ###
            //############################################################
            //############################################################

            //###################### coloring start ######################
            if (KPIScore <= (targetBenchmark - tenPercentOfTarget))
            {
                KPIScoreColor = "#C1272D";//red
                chape = "weave";
            }
            else if (KPIScore < targetBenchmark)
            {
                KPIScoreColor = "#F7931E";//yellow
                chape = "cross";
            }
            else if (KPIScore >= (targetBenchmark + tenPercentOfTarget))
            {
                KPIScoreColor = "#29B8BE";//blue
                chape = "dot";
            }
            else
            {
                KPIScoreColor = "#71BA44";//green
                chape = "line-vertical";
            }

            if (gender != "0")
            {
                if (gender.Equals("انثى"))
                {
                    fColor = "#1F3771";//dark yellow
                    mColor = "#B0AEB3";//light grey
                }
                else
                {
                    mColor = "#1F3771";//dark blue
                    fColor = "#B0AEB3";//light grey
                }
            }
            //###################### coloring end ########################


            //############################################################
            //############################################################
            //     send the data to the View to draw the charts        ###
            //############################################################
            //############################################################

            //################### four value KPI-11-13 chart data start ##
            data.Add(new Survey //0
            {
                Gender = "القيمة الفعلية",
                Nationality = KPIScoreColor,
                Term = "" + KPIScore,
                StudentCase = progName,
                Year = chape
            });
            data.Add(new Survey //1
            {
                Gender = "المقارنة المرجعية الخارجية",
                Nationality = "#D0DA32",
                Term = "" + externalBenchmark
            });
            data.Add(new Survey //2
            {
                Gender = "المقارنة المرجعية الداخلية",
                Nationality = "#B0AEB3",
                Term = "" + internalBenchmark
            });
            data.Add(new Survey //3
            {
                Gender = "الاداء المستهدف",
                Nationality = "#1F3771",
                Term = "" + targetBenchmark
            });
            //################### four value KPI-11-13 chart data end ####

            //################### gender pie KPI-12 chart data start #####
            data.Add(new Survey //4
            {
                Gender = "انثى",
                Nationality = fColor,
                Term = "" + chart1.percentage(2, countKPI_12_Female, countKPI_12_Male, 0, 0, 0, 0),
                ProgramId = (int)(countKPI_12_Female)
            });
            data.Add(new Survey //5
            {
                Gender = "ذكر",
                Nationality = mColor,
                Term = "" + chart1.percentage(2, countKPI_12_Male, countKPI_12_Female, 0, 0, 0, 0),
                ProgramId = (int)(countKPI_12_Male)
            });
            //################### gender pie KPI-12 chart data end #######

            //############ position pie KPI-12 chart data end #####
            data.Add(new Survey //6
            {
                Gender = "مدرس",
                Term = "" + chart1.percentage(6, countKPI_12_FemaleTeacher, countKPI_12_FemaleTeachingAssistant, countKPI_12_FemaleLecturer, countKPI_12_FemaleAssistantPorf, countKPI_12_FemaleAssociateProf, countKPI_12_FemaleProf),
                ProgramId = (int)(countKPI_12_FemaleTeacher)
            });
            data.Add(new Survey //7
            {
                Gender = "معيد",
                Term = "" + chart1.percentage(6, countKPI_12_FemaleTeachingAssistant, countKPI_12_FemaleTeacher, countKPI_12_FemaleLecturer, countKPI_12_FemaleAssistantPorf, countKPI_12_FemaleAssociateProf, countKPI_12_FemaleProf),
                ProgramId = (int)(countKPI_12_FemaleTeachingAssistant)
            });
            data.Add(new Survey //8
            {
                Gender = "محاضر",
                Term = "" + chart1.percentage(6, countKPI_12_FemaleLecturer, countKPI_12_FemaleTeachingAssistant, countKPI_12_FemaleTeacher, countKPI_12_FemaleAssistantPorf, countKPI_12_FemaleAssociateProf, countKPI_12_FemaleProf),
                ProgramId = (int)(countKPI_12_FemaleLecturer)
            });
            data.Add(new Survey //9
            {
                Gender = "استاذ مساعد",
                Term = "" + chart1.percentage(6, countKPI_12_FemaleAssistantPorf, countKPI_12_FemaleTeachingAssistant, countKPI_12_FemaleLecturer, countKPI_12_FemaleTeacher, countKPI_12_FemaleAssociateProf, countKPI_12_FemaleProf),
                ProgramId = (int)(countKPI_12_FemaleAssistantPorf)
            });
            data.Add(new Survey //10
            {
                Gender = "استاذ مشارك",
                Term = "" + chart1.percentage(6, countKPI_12_FemaleAssociateProf, countKPI_12_FemaleTeachingAssistant, countKPI_12_FemaleLecturer, countKPI_12_FemaleAssistantPorf, countKPI_12_FemaleTeacher, countKPI_12_FemaleProf),
                ProgramId = (int)(countKPI_12_FemaleAssociateProf)
            });
            data.Add(new Survey //11
            {
                Gender = "بروف",
                Term = "" + chart1.percentage(6, countKPI_12_FemaleProf, countKPI_12_FemaleTeachingAssistant, countKPI_12_FemaleLecturer, countKPI_12_FemaleAssistantPorf, countKPI_12_FemaleAssociateProf, countKPI_12_FemaleTeacher),
                ProgramId = (int)(countKPI_12_FemaleProf)
            });
            //############ position pie KPI-12 chart data end #####

            //############ Female position pie KPI-12 chart data end #####
            data.Add(new Survey //12
            {
                Gender = "مدرس",
                Nationality = fColor,
                Term = "" + chart1.percentage(2, countKPI_12_FemaleTeacher, countKPI_12_MaleTeacher, 0, 0, 0, 0),
                ProgramId = (int)(countKPI_12_FemaleTeacher)
            });
            data.Add(new Survey //13
            {
                Gender = "معيد",
                Nationality = fColor,
                Term = "" + chart1.percentage(2, countKPI_12_FemaleTeachingAssistant, countKPI_12_MaleTeachingAssistant, 0, 0, 0, 0),
                ProgramId = (int)(countKPI_12_FemaleTeachingAssistant)
            });
            data.Add(new Survey //14
            {
                Gender = "محاضر",
                Nationality = fColor,
                Term = "" + chart1.percentage(2, countKPI_12_FemaleLecturer, countKPI_12_MaleLecturer, 0, 0, 0, 0),
                ProgramId = (int)(countKPI_12_FemaleLecturer)
            });
            data.Add(new Survey //15
            {
                Gender = "استاذ مساعد",
                Nationality = fColor,
                Term = "" + chart1.percentage(2, countKPI_12_FemaleAssistantPorf, countKPI_12_MaleAssistantPorf, 0, 0, 0, 0),
                ProgramId = (int)(countKPI_12_FemaleAssistantPorf)
            });
            data.Add(new Survey //16
            {
                Gender = "استاذ مشارك",
                Nationality = fColor,
                Term = "" + chart1.percentage(2, countKPI_12_FemaleAssociateProf, countKPI_12_MaleAssociateProf, 0, 0, 0, 0),
                ProgramId = (int)(countKPI_12_FemaleAssociateProf)
            });
            data.Add(new Survey //17
            {
                Gender = "بروف",
                Nationality = fColor,
                Term = "" + chart1.percentage(2, countKPI_12_FemaleProf, countKPI_12_MaleProf, 0, 0, 0, 0),
                ProgramId = (int)(countKPI_12_FemaleProf)
            });
            //############ Female position pie KPI-12 chart data end #####

            //############ Male position pie KPI-12 chart data end #######
            data.Add(new Survey //18
            {
                Gender = "مدرس",
                Nationality = mColor,
                Term = "" + chart1.percentage(2, countKPI_12_MaleTeacher, countKPI_12_FemaleTeacher, 0, 0, 0, 0),
                ProgramId = (int)(countKPI_12_MaleTeacher)
            });
            data.Add(new Survey //19
            {
                Gender = "معيد",
                Nationality = mColor,
                Term = "" + chart1.percentage(2, countKPI_12_MaleTeachingAssistant, countKPI_12_FemaleTeachingAssistant, 0, 0, 0, 0),
                ProgramId = (int)(countKPI_12_MaleTeachingAssistant)
            });
            data.Add(new Survey //20
            {
                Gender = "محاضر",
                Nationality = mColor,
                Term = "" + chart1.percentage(2, countKPI_12_MaleLecturer, countKPI_12_FemaleLecturer, 0, 0, 0, 0),
                ProgramId = (int)(countKPI_12_MaleLecturer)
            });
            data.Add(new Survey //21
            {
                Gender = "استاذ مساعد",
                Nationality = mColor,
                Term = "" + chart1.percentage(2, countKPI_12_MaleAssistantPorf, countKPI_12_FemaleAssistantPorf, 0, 0, 0, 0),
                ProgramId = (int)(countKPI_12_MaleAssistantPorf)
            });
            data.Add(new Survey //22
            {
                Gender = "استاذ مشارك",
                Nationality = mColor,
                Term = "" + chart1.percentage(2, countKPI_12_MaleAssociateProf, countKPI_12_FemaleAssociateProf, 0, 0, 0, 0),
                ProgramId = (int)(countKPI_12_MaleAssociateProf)
            });
            data.Add(new Survey //23
            {
                Gender = "بروف",
                Nationality = mColor,
                Term = "" + chart1.percentage(2, countKPI_12_MaleProf, countKPI_12_FemaleProf, 0, 0, 0, 0),
                ProgramId = (int)(countKPI_12_MaleProf)
            });
            //############ Male position pie KPI-12 chart data end #####

            //################### gender pie KPI-13 chart data start #####
            data.Add(new Survey //24
            {
                Gender = "انثى",
                Nationality = fColor,
                Term = "" + chart1.percentage(2, countKPI_13_FemaleResignation, countKPI_13_MaleResignation, 0, 0, 0, 0),
                ProgramId = (int)(countKPI_13_FemaleResignation)
            });
            data.Add(new Survey //25
            {
                Gender = "ذكر",
                Nationality = mColor,
                Term = "" + chart1.percentage(2, countKPI_13_MaleResignation, countKPI_13_FemaleResignation, 0, 0, 0, 0),
                ProgramId = (int)(countKPI_13_MaleResignation)
            });
            //################### gender pie KPI-13 chart data end #######

            return Json(data);

        }
        // KPI-P-11-13 charts code end


        //KPI-P-14-16 charts code start
        [HttpGet]
        public ActionResult GetPublicationReportsChart(String programId, String KPICode, String gender, String year)
        {
            charts chart1 = new charts(_db);
            var kpi = _db.Kpis.FirstOrDefault(x => x.Kpicode == KPICode);
            String progName = chart1.progName(programId);

            //create a list for each chart
            var publication = _db.PublicationReports.Where(x => x.ProgramId == Convert.ToInt32(programId) && x.Year == year).ToList();
            var faculty = _db.FacultyStatistics.Where(x => x.ProgramId == Convert.ToInt32(programId) && x.Year == year).ToList();
            var KPIResult = _db.Kpiprograms.Where(x => x.KpiId == kpi.KpiId && x.ProgramId == Convert.ToInt32(programId) && x.Year == year).ToList();
            KPIResult = chart1.filterKPIList(KPIResult, null, gender);

            var NumOfFemaleFaculty = chart1.PublicationScore(publication, faculty, 'f', "انثى");
            var NumOfMaleFaculty = chart1.PublicationScore(publication, faculty, 'f', "ذكر");
            var numOfFaculty = NumOfFemaleFaculty + NumOfMaleFaculty;
            var publicationsNumF = chart1.PublicationScore(publication, faculty, 'p', "انثى");
            var publicationsNumM = chart1.PublicationScore(publication, faculty, 'p', "ذكر");
            var numOfFacultyWithOnePublicationF = chart1.PublicationScore(publication, faculty, 'o', "انثى");
            var numOfFacultyWithOnePublicationM = chart1.PublicationScore(publication, faculty, 'o', "ذكر");
            var numOfCitation = chart1.PublicationScore(publication, faculty, 'c', gender);

            //declare the result and the bars color for each chart
            String fColor = "#D0DA32", mColor = "#1F3771"; String KPIScoreColor;
            var KPIScore = 0.0; float internalBenchmark = chart1.KPIscore(KPIResult, 'i'), externalBenchmark = chart1.KPIscore(KPIResult, 'e'), targetBenchmark = chart1.KPIscore(KPIResult, 'k'); var tenPercentOfTarget = targetBenchmark * .10;


            if (KPICode.Equals("KPI-P-14"))
                KPIScore = (float)(((numOfFacultyWithOnePublicationF + numOfFacultyWithOnePublicationM) / numOfFaculty) * 100);

            else if (KPICode.Equals("KPI-P-15"))
                KPIScore = (float)(((publicationsNumF + publicationsNumM) / numOfFaculty) * 100);

            else if (KPICode.Equals("KPI-P-16"))
                KPIScore = (float)((numOfCitation / (publicationsNumF + publicationsNumM)) * 100);

            //############################################################
            //############################################################
            // color the chart bars based on the chosen filter options ###
            //############################################################
            //############################################################

            //###################### coloring start ######################
            if (gender != "0")
            {
                if (gender.Equals("انثى"))
                {
                    fColor = "#1F3771";//dark yellow
                    mColor = "#B0AEB3";//light grey
                }
                else
                {
                    mColor = "#1F3771";//dark blue
                    fColor = "#B0AEB3";//light grey
                }
            }

            //coloring the KPI result based on the target 
            if (KPIScore <= (targetBenchmark - tenPercentOfTarget))
            {
                KPIScoreColor = "#C1272D";//red
                chape = "weave";
            }
            else if (KPIScore < targetBenchmark)
            {
                KPIScoreColor = "#F7931E";//yellow
                chape = "cross";
            }
            else if (KPIScore >= (targetBenchmark + tenPercentOfTarget))
            {
                KPIScoreColor = "#29B8BE";//blue
                chape = "dot";
            }
            else
            {
                KPIScoreColor = "#71BA44";//green
                chape = "line-vertical";
            }
            //###################### coloring end ########################

            //############################################################
            //############################################################
            //     send the data to the View to draw the charts        ###
            //############################################################
            //############################################################

            //################### number of faculty chart data start #####
            data.Add(new Survey
            {
                Gender = "انثى",
                Nationality = fColor,
                Term = "" + chart1.percentage(2, numOfFacultyWithOnePublicationF, numOfFacultyWithOnePublicationM, 0, 0, 0, 0),
                StudentCase = progName, //program Name
                ProgramId = (int)numOfFacultyWithOnePublicationF

            });
            data.Add(new Survey
            {
                Gender = "ذكر",
                Nationality = mColor,
                Term = "" + chart1.percentage(2, numOfFacultyWithOnePublicationM, numOfFacultyWithOnePublicationF, 0, 0, 0, 0),
                StudentCase = progName, //program Name
                ProgramId = (int)numOfFacultyWithOnePublicationM

            });
            //################### number of faculty chart data end #######

            //################### number of publication chart data start #
            data.Add(new Survey
            {
                Gender = "انثى",
                Nationality = fColor,
                Term = "" + chart1.percentage(2, publicationsNumF, publicationsNumM, 0, 0, 0, 0),
                StudentCase = progName, //program Name
                ProgramId = (int)publicationsNumF

            });
            data.Add(new Survey
            {
                Gender = "ذكر",
                Nationality = mColor,
                Term = "" + chart1.percentage(2, publicationsNumM, publicationsNumF, 0, 0, 0, 0),
                StudentCase = progName, //program Name
                ProgramId = (int)publicationsNumM

            });
            //################### number of publication chart data end ###

            //################### four values chart data start ###########
            data.Add(new Survey
            {
                Gender = "القيمة الفعلية",
                Nationality = KPIScoreColor,
                Term = "" + KPIScore,
                StudentCase = progName, //program Name
                Year = chape
            });
            data.Add(new Survey
       {
           Gender = "المقارنة المرجعية الخارجية",
           Nationality = "#D0DA32",
           Term = "" + externalBenchmark,
           StudentCase = progName
       });
            data.Add(new Survey
            {
                Gender = "المقارنة المرجعية الداخلية",
                Nationality = "#B0AEB3",
                Term = "" + internalBenchmark,
                StudentCase = progName
            });
            data.Add(new Survey
            {
                Gender = "الاداء المستهدف",
                Nationality = "#1F3771",
                Term = "" + targetBenchmark,
                StudentCase = progName
            });
            //################### four values chart data end #############
            return Json(data);
        }
        // KPI-P-14-16 charts code end


        // charts table code start
        [HttpGet]
        public ActionResult GettableValues(String programId, String KPICode, float internalBenchmark, float externalBenchmark, float newTargetBenchmark, float targetBenchmark, float actualKpivalue, String term, String year, String gender)
        {
            var kpi = _db.Kpis.Where(x => x.Kpiname == KPICode).ToList();
            List<Kpiprogram> PKList = new List<Kpiprogram>();
            var pk = _db.Kpiprograms.Where(x => x.ProgramId == Convert.ToInt32(programId) && x.Year == year).ToList();

            if (KPICode == "مؤشرات الاستبانات" || KPICode == "مؤشرات البحوث" || KPICode == "مؤشر الفصول")
            {
                if (term != "0")
                {
                    pk = pk.Where(x => x.Term == term).ToList();
                }
            }
            var count = 0;
            foreach (var row in kpi)
                count++;
            if (count == 1)
                pk = pk.Where(x => x.KpiId == kpi.ElementAt(0).KpiId).ToList();
            if (count == 2)
                pk = pk.Where(x => x.KpiId == kpi.ElementAt(0).KpiId || x.KpiId == kpi.ElementAt(1).KpiId).ToList();
            if (count == 3)
                pk = pk.Where(x => x.KpiId == kpi.ElementAt(0).KpiId || x.KpiId == kpi.ElementAt(1).KpiId || x.KpiId == kpi.ElementAt(2).KpiId).ToList();
            if (count == 4)
                pk = pk.Where(x => x.KpiId == kpi.ElementAt(0).KpiId || x.KpiId == kpi.ElementAt(1).KpiId || x.KpiId == kpi.ElementAt(2).KpiId || x.KpiId == kpi.ElementAt(3).KpiId).ToList();

            // add to list 
            foreach (var item in pk)
            {
                PKList.Add(new Kpiprogram
                {
                    InternalBenchmark = item.InternalBenchmark,
                    ExternalBenchmark = item.ExternalBenchmark,
                    TargetBenchmark = item.TargetBenchmark,
                    ActualKpivalue = item.ActualKpivalue,
                    Term = (_db.Kpis.FirstOrDefault(x => x.KpiId == item.KpiId)).Kpicode,
                    Year= item.Term,
                    Gender = _db.Kpis.FirstOrDefault(x => x.KpiId == item.KpiId).Description
                });

            }

            return Json(PKList);

        }
       
        // charts table code end
    }

}
