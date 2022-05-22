using IP_KPI.Data;
using IP_KPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace IP_KPI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
     
        private readonly db_a7baa5_ipkpiContext _context;
        
        public AdminController(db_a7baa5_ipkpiContext context)
        {
            _context = context;
        }

        public void PrivilegesNotification()
        {
            var n = _context.PrivilegeRequests;
            var count = 0;
            foreach (var item in n)
                count++;
            ViewData["privilegeCount"] = count;
        }
        public async Task<IActionResult> Privileges()
        {
            PrivilegesNotification();
            var db_a7baa5_ipkpiContext = _context.PrivilegeRequests;
            return View(await db_a7baa5_ipkpiContext.ToListAsync());
        }

        //###########################################################################################################
        //###########################################################################################################
        //departments//
        // GET: Departments
        public async Task<IActionResult> DepartmentsIndex()
        {
            PrivilegesNotification();
            var db_a7baa5_ipkpiContext = _context.Departments.Include(d => d.College);
            return View(await db_a7baa5_ipkpiContext.ToListAsync());
        }

        // GET: Departments/Create
        public IActionResult DepartmentsCreate()
        {
            PrivilegesNotification();
            ViewData["CollegeId"] = new SelectList(_context.Colleges, "CollegeId", "CollegeId");
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DepartmentsCreate([Bind("DepartmentId,DepartmentName,CollegeId")] Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(DepartmentsIndex));
            }
            ViewData["CollegeId"] = new SelectList(_context.Colleges, "CollegeId", "CollegeId", department.CollegeId);
            return View(department);
        }

        // GET: Departments/Edit/5
        public async Task<IActionResult> DepartmentsEdit(int? id)
        {
            PrivilegesNotification();
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            ViewData["CollegeId"] = new SelectList(_context.Colleges, "CollegeId", "CollegeId", department.CollegeId);
            return View(department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DepartmentsEdit(int id, [Bind("DepartmentId,DepartmentName,CollegeId")] Department department)
        {
            if (id != department.DepartmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(department);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.DepartmentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(DepartmentsIndex));
            }
            ViewData["CollegeId"] = new SelectList(_context.Colleges, "CollegeId", "CollegeId", department.CollegeId);
            return View(department);
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.DepartmentId == id);
        }

        [HttpPost]
        public async Task<IActionResult> deleteDepartment(int id)
        {
            var dep = await _context.Departments.FindAsync(id);
            _context.Departments.Remove(dep);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DepartmentsIndex));
        }




        //###########################################################################################################
        //###########################################################################################################
        //publictions// 
        // GET: PublicationReports
        public async Task<IActionResult> PublicationReportsIndex()
        {
            PrivilegesNotification();
            var db_a7baa5_ipkpiContext = _context.PublicationReports.Include(p => p.Program);
            return View(await db_a7baa5_ipkpiContext.ToListAsync());
        }

        // GET: PublicationReports/Edit/5
        public async Task<IActionResult> PublicationReportsEdit(int? id)
        {
            PrivilegesNotification();
            if (id == null)
            {
                return NotFound();
            }

            var publicationReport = await _context.PublicationReports.FindAsync(id);
            if (publicationReport == null)
            {
                return NotFound();
            }
            ViewData["ProgramId"] = new SelectList(_context.UniPrograms, "ProgramId", "ProgramId", publicationReport.ProgramId);
            return View(publicationReport);
        }
        //#################################################################################################################
        // POST: PublicationReports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PublicationReportsEdit(int id, [Bind("PublicationReportId,Gender,NumOfFacultyOneP,NumOfPublications,NumOfCitations,Year,ProgramId")] PublicationReport publicationReport)
        {
            if (id != publicationReport.PublicationReportId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(publicationReport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PublicationReportExists(publicationReport.PublicationReportId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(PublicationReportsIndex));
            }
            ViewData["ProgramId"] = new SelectList(_context.UniPrograms, "ProgramId", "ProgramId", publicationReport.ProgramId);
            return View(publicationReport);
        }

       
        private bool PublicationReportExists(int id)
        {
            return _context.PublicationReports.Any(e => e.PublicationReportId == id);
        }

        [HttpPost]
        public async Task<IActionResult> deletePublicationReport(int id)
        {
            var pr = await _context.PublicationReports.FindAsync(id);
            _context.PublicationReports.Remove(pr);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(PublicationReportsIndex));
        }


        //###########################################################################################################
        //###########################################################################################################
        //Colleges// 
        // GET: Colleges
        public async Task<IActionResult> Index()
        {
            PrivilegesNotification();
            return View(await _context.Colleges.ToListAsync());
        }

        // GET: Colleges/Create
        public IActionResult CollegesCreate()
        {
            PrivilegesNotification();
            return View();
        }

        // POST: Colleges/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CollegesCreate([Bind("CollegeId,CollageName")] College college)
        {
            if (ModelState.IsValid)
            {
                _context.Add(college);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(college);
        }

        // GET: Colleges/Edit/5
        public async Task<IActionResult> CollegesEdit(int? id)
        {
            PrivilegesNotification();
            if (id == null)
            {
                return NotFound();
            }

            var college = await _context.Colleges.FindAsync(id);
            if (college == null)
            {
                return NotFound();
            }
            return View(college);
        }

        // POST: Colleges/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CollegesEdit(int id, [Bind("CollegeId,CollageName")] College college)
        {
            if (id != college.CollegeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(college);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CollegeExists(college.CollegeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(college);
        }


       
        private bool CollegeExists(int id)
        {
            return _context.Colleges.Any(e => e.CollegeId == id);
        }

        [HttpPost]
        public async Task<IActionResult> deleteCollege(int id)
        {
            var col = await _context.Colleges.FindAsync(id);
            _context.Colleges.Remove(col);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        
        
        
        //###########################################################################################################
        //###########################################################################################################
        //Survey// 
        // GET: Surveys
        public async Task<IActionResult> SurveysIndex()
        {
            PrivilegesNotification();
            var db_a7baa5_ipkpiContext = _context.Surveys.Include(s => s.Kpi).Include(s => s.Program);
            return View(await db_a7baa5_ipkpiContext.ToListAsync());
        }

        // GET: Surveys/Edit/5
        public async Task<IActionResult> SurveysEdit(int? id)
        {
            PrivilegesNotification();
            if (id == null)
            {
                return NotFound();
            }

            var survey = await _context.Surveys.FindAsync(id);
            if (survey == null)
            {
                return NotFound();
            }
            ViewData["KpiId"] = new SelectList(_context.Kpis, "KpiId", "KpiId", survey.KpiId);
            ViewData["ProgramId"] = new SelectList(_context.UniPrograms, "ProgramId", "ProgramId", survey.ProgramId);
            return View(survey);
        }

        // POST: Surveys/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SurveysEdit(int id, [Bind("RecordNumber,Term,NumberOfStudent,Year,ExtraActivity,StudentCase,Nationality,Gender,NumOfRespondent,SurveyScore,KpiId,ProgramId")] Survey survey)
        {
            if (id != survey.RecordNumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(survey);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SurveyExists(survey.RecordNumber))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(SurveysIndex));
            }
            ViewData["KpiId"] = new SelectList(_context.Kpis, "KpiId", "KpiId", survey.KpiId);
            ViewData["ProgramId"] = new SelectList(_context.UniPrograms, "ProgramId", "ProgramId", survey.ProgramId);
            return View(survey);
        }


        private bool SurveyExists(int id)
        {
            return _context.Surveys.Any(e => e.RecordNumber == id);
        }

        [HttpPost]
        public async Task<IActionResult> deleteSurvey(int id)
        {
            var ss = await _context.Surveys.FindAsync(id);
            _context.Surveys.Remove(ss);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(SurveysIndex));
        }


        //###########################################################################################################
        //###########################################################################################################
        //BatchStatstics// 
        // GET: BatchStatistics
        public async Task<IActionResult> BatchStatisticsIndex()
        {
            PrivilegesNotification();
            var db_a7baa5_ipkpiContext = _context.BatchStatistics.Include(b => b.Program);
            return View(await db_a7baa5_ipkpiContext.ToListAsync());
        }


        // GET: BatchStatistics/Edit/5
        public async Task<IActionResult> BatchStatisticsEdit(int? id)
        {
            PrivilegesNotification();
            if (id == null)
            {
                return NotFound();
            }

            var batchStatistic = await _context.BatchStatistics.FindAsync(id);
            if (batchStatistic == null)
            {
                return NotFound();
            }
            ViewData["ProgramId"] = new SelectList(_context.UniPrograms, "ProgramId", "ProgramId", batchStatistic.ProgramId);
            return View(batchStatistic);
        }

        // POST: BatchStatistics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BatchStatisticsEdit(int id, [Bind("Year,Gender,NumOfBatchStudent,NumMinYearStudent,NumFirstYearStudent,NumStudentContinue,BatchId,ProgramId")] BatchStatistic batchStatistic)
        {
            if (id != batchStatistic.BatchId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(batchStatistic);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BatchStatisticExists(batchStatistic.BatchId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(BatchStatisticsIndex));
            }
            ViewData["ProgramId"] = new SelectList(_context.UniPrograms, "ProgramId", "ProgramId", batchStatistic.ProgramId);
            return View(batchStatistic);
        }

        private bool BatchStatisticExists(int id)
        {
            return _context.BatchStatistics.Any(e => e.BatchId == id);
        }
        [HttpPost]
        public async Task<IActionResult> deleteBatchstatistic(int id)
        {
            var bs = await _context.BatchStatistics.FindAsync(id);
            _context.BatchStatistics.Remove(bs);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(BatchStatisticsIndex));
        }




        //###########################################################################################################
        //###########################################################################################################
        //Users and Privileges//
        // GET: Users/Edit/5

        public async Task<IActionResult> UserIndex()
        {
            PrivilegesNotification();
            return View(await _context.Users.ToListAsync());
        }

        public async Task<IActionResult> EditUser(String id)
        {
            PrivilegesNotification();
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                User newUser = new User();
                newUser.UserId = id;
                newUser.Privilege = "";
                return View(newUser);
            }
            return View(user);
        }

        private bool UserExists(String id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

     
       
        public IActionResult UsersCreate()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UsersCreate([Bind("UserId,Privilege,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(UserIndex));
            }
            return View(user);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(String id, [Bind("UserId,Privilege,Password")] User user)
        {
            if (id != user.UserId)
            {
                    return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (UserExists(user.UserId))
                    {
                        _context.Update(user);
                    }
                    else
                    {
                        _context.Add(user);

                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                var endedRequest = _context.PrivilegeRequests.Where(x => x.UserId == id).ToList();
                foreach(var item in endedRequest)
                _context.PrivilegeRequests.Remove(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(UserIndex));
            }
            return View(user);
        }


        public async Task<IActionResult> deletePrivilegeRequest(int id)
        {
            var col = await _context.PrivilegeRequests.FindAsync(id);
            _context.PrivilegeRequests.Remove(col);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Privileges));
        }

        [HttpPost]
        public async Task<IActionResult> deleteUser(String id)
        {
            var bs = await _context.Users.FindAsync(id);
            _context.Users.Remove(bs);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(UserIndex));
        }


        //###########################################################################################################
        //###########################################################################################################
        //ClassSection//
        // GET: ClassSections
        public async Task<IActionResult> ClassSectionsIndex()
        {
            PrivilegesNotification();
            var db_a7baa5_ipkpiContext = _context.ClassSections.Include(c => c.Program);
            return View(await db_a7baa5_ipkpiContext.ToListAsync());
        }

        // GET: ClassSections/Edit/5
        public async Task<IActionResult> ClassSectionsEdit(int? id)
        {
            PrivilegesNotification();
            if (id == null)
            {
                return NotFound();
            }

            var classSection = await _context.ClassSections.FindAsync(id);
            if (classSection == null)
            {
                return NotFound();
            }
            ViewData["ProgramId"] = new SelectList(_context.UniPrograms, "ProgramId", "ProgramId", classSection.ProgramId);
            return View(classSection);
        }

        // POST: ClassSections/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClassSectionsEdit(int id, [Bind("Year,Gender,Term,NumOfClasses,SectionId,ProgramId")] ClassSection classSection)
        {
            if (id != classSection.SectionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(classSection);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassSectionExists(classSection.SectionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ClassSectionsIndex));
            }
            ViewData["ProgramId"] = new SelectList(_context.UniPrograms, "ProgramId", "ProgramId", classSection.ProgramId);
            return View(classSection);
        }

        

        private bool ClassSectionExists(int id)
        {
            return _context.ClassSections.Any(e => e.SectionId == id);

        }
        [HttpPost]
        public async Task<IActionResult> deleteClassSection(int id)
        {
            var cs = await _context.ClassSections.FindAsync(id);
            _context.ClassSections.Remove(cs);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ClassSectionsIndex));
        }


        //###########################################################################################################
        //###########################################################################################################
        //KPIs//
        // GET: Kpis
        public async Task<IActionResult> KpisIndex()
        {
            PrivilegesNotification();
            var db_a7baa5_ipkpiContext = _context.Kpis;
            return View(await db_a7baa5_ipkpiContext.ToListAsync());
        }



        // GET: Kpis/Create
        public IActionResult KpisCreate()
        {
            PrivilegesNotification();
            ViewData["FacultyId"] = new SelectList(_context.PublicationReports, "PublicationReportId", "PublicationReportId");
            return View();
        }

        // POST: Kpis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
      
        // GET: Kpis/Edit/5
        public async Task<IActionResult> KpisEdit(int? id)
        {
            PrivilegesNotification();
            if (id == null)
            {
                return NotFound();
            }

            var kpi = await _context.Kpis.FindAsync(id);
            if (kpi == null)
            {
                return NotFound();
            }
            ViewData["FacultyId"] = new SelectList(_context.PublicationReports, "PublicationReportId", "PublicationReportId");
            return View(kpi);
        }

        // POST: Kpis/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KpisEdit(int id, [Bind("KpiId,Kpicode,Kpiname")] Kpi kpi)
        {
            if (id != kpi.KpiId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(kpi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KpiExists(kpi.KpiId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(KpisIndex));
            }
            ViewData["FacultyId"] = new SelectList(_context.PublicationReports, "PublicationReportId", "PublicationReportId");
            return View(kpi);
        }

        private bool KpiExists(int id)
        {
            return _context.Kpis.Any(e => e.KpiId == id);
        }
        [HttpPost]
        public async Task<IActionResult> deleteKpi(int id)
        {
            var k = await _context.Kpis.FindAsync(id);
            _context.Kpis.Remove(k);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(KpisIndex));
        }


        //###########################################################################################################
        //###########################################################################################################
        //Kpiprograms//
        // GET: Kpiprograms
        public async Task<IActionResult> KpiprogramsIndex()
        {
            PrivilegesNotification();
            var db_a7baa5_ipkpiContext = _context.Kpiprograms.Include(k => k.Kpi).Include(k => k.Program);
            return View(await db_a7baa5_ipkpiContext.ToListAsync());
        }


        // GET: Kpiprograms/Edit/5
        public async Task<IActionResult> KpiprogramsEdit(int? id)
        {
            PrivilegesNotification();
            if (id == null)
            {
                return NotFound();
            }

            var kpiprogram = await _context.Kpiprograms.FindAsync(id);
            if (kpiprogram == null)
            {
                return NotFound();
            }
            ViewData["KpiId"] = new SelectList(_context.Kpis, "KpiId", "KpiId", kpiprogram.KpiId);
            ViewData["ProgramId"] = new SelectList(_context.UniPrograms, "ProgramId", "ProgramId", kpiprogram.ProgramId);
            return View(kpiprogram);
        }

        // POST: Kpiprograms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KpiprogramsEdit(int id, [Bind("KpiprogramId,KpiId,ProgramId,TargetBenchmark,NewTargetBenchmark,InternalBenchmark,ExternalBenchmark,Year,Term,Gender,ActualKpivalue")] Kpiprogram kpiprogram)
        {
            if (id != kpiprogram.KpiprogramId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(kpiprogram);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KpiprogramExists(kpiprogram.KpiprogramId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(KpiprogramsIndex));
            }
            ViewData["KpiId"] = new SelectList(_context.Kpis, "KpiId", "KpiId", kpiprogram.KpiId);
            ViewData["ProgramId"] = new SelectList(_context.UniPrograms, "ProgramId", "ProgramId", kpiprogram.ProgramId);
            return View(kpiprogram);
        }

        
        private bool KpiprogramExists(int id)
        {
            return _context.Kpiprograms.Any(e => e.KpiprogramId == id);
        }

        [HttpPost]
        public async Task<IActionResult> deleteKpiprogram(int id)
        {
            var col = await _context.Kpiprograms.FindAsync(id);
            _context.Kpiprograms.Remove(col);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(KpiprogramsIndex));
        }


        //###########################################################################################################
        //###########################################################################################################
        //UniPrograms//
        // GET: UniPrograms
        public async Task<IActionResult> UniProgramsIndex()
        {
            PrivilegesNotification();
            var db_a7baa5_ipkpiContext = _context.UniPrograms.Include(u => u.Department);
            return View(await db_a7baa5_ipkpiContext.ToListAsync());
        }
       


        // GET: UniPrograms/Create
        public IActionResult UniProgramsCreate()
        {
            PrivilegesNotification();
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId");
            return View();
        }

        // POST: UniPrograms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UniProgramsCreate([Bind("ProgramId,ProgramName,DepartmentId,Level")] UniProgram uniProgram)
        {
            if (ModelState.IsValid)
            {
                _context.Add(uniProgram);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(UniProgramsIndex));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", uniProgram.DepartmentId);
            return View(uniProgram);
        }

        // GET: UniPrograms/Edit/5
        public async Task<IActionResult> UniProgramsEdit(int? id)
        {
            PrivilegesNotification();
            if (id == null)
            {
                return NotFound();
            }

            var uniProgram = await _context.UniPrograms.FindAsync(id);
            if (uniProgram == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", uniProgram.DepartmentId);
            return View(uniProgram);
        }

        // POST: UniPrograms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UniProgramsEdit(int id, [Bind("ProgramId,ProgramName,DepartmentId,Level")] UniProgram uniProgram)
        {
            if (id != uniProgram.ProgramId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(uniProgram);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UniProgramExists(uniProgram.ProgramId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(UniProgramsIndex));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", uniProgram.DepartmentId);
            return View(uniProgram);
        }

        
        private bool UniProgramExists(int id)
        {
            return _context.UniPrograms.Any(e => e.ProgramId == id);
        }

        [HttpPost]
        public async Task<IActionResult> deleteUniProgram(int id)
        {
            var up = await _context.UniPrograms.FindAsync(id);
            _context.UniPrograms.Remove(up);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(UniProgramsIndex));
        }

        //###########################################################################################################
        //###########################################################################################################
        //FacultyStatistics//
        // GET: FacultyStatistics
        public async Task<IActionResult> FacultyStatisticsIndex()
        {
            PrivilegesNotification();
            var db_a7baa5_ipkpiContext = _context.FacultyStatistics.Include(f => f.Program);
            return View(await db_a7baa5_ipkpiContext.ToListAsync());
        }

        // GET: FacultyStatistics/Edit/5
        public async Task<IActionResult> FacultyStatisticsEdit(int? id)
        {
            PrivilegesNotification();
            if (id == null)
            {
                return NotFound();
            }

            var facultyStatistic = await _context.FacultyStatistics.FindAsync(id);
            if (facultyStatistic == null)
            {
                return NotFound();
            }
            ViewData["ProgramId"] = new SelectList(_context.UniPrograms, "ProgramId", "ProgramId", facultyStatistic.ProgramId);
            return View(facultyStatistic);
        }

        // POST: FacultyStatistics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FacultyStatisticsEdit(int id, [Bind("FacultyId,Year,Gender,NumOfAssistantPorf,NumOfProf,NumOfAssociateProf,NumOfTeacher,NumOfLecturer,NumOfTeachingAssistant,NumOfResignation,ProgramId,Type")] FacultyStatistic facultyStatistic)
        {
            if (id != facultyStatistic.FacultyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(facultyStatistic);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacultyStatisticExists(facultyStatistic.FacultyId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(FacultyStatisticsIndex));
            }
            ViewData["ProgramId"] = new SelectList(_context.UniPrograms, "ProgramId", "ProgramId", facultyStatistic.ProgramId);
            return View(facultyStatistic);
        }

       

        private bool FacultyStatisticExists(int id)
        {
            return _context.FacultyStatistics.Any(e => e.FacultyId == id);
        }

        [HttpPost]
        public async Task<IActionResult> deleteFacultyStatistic(int id)
        {
            var fs = await _context.FacultyStatistics.FindAsync(id);
            _context.FacultyStatistics.Remove(fs);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(FacultyStatisticsIndex));
        }

        // GET: AlumniEmployments
        public async Task<IActionResult> AlumniEmploymentsIndex()
        {
            PrivilegesNotification();

            var db_a7baa5_ipkpiContext = _context.AlumniEmployments.Include(a => a.Program);
            return View(await db_a7baa5_ipkpiContext.ToListAsync());
        }

        // GET: AlumniEmployments/Edit/5
        public async Task<IActionResult> AlumniemploymentsEdit(int? id)
        {
            PrivilegesNotification();

            if (id == null)
            {
                return NotFound();
            }

            var alumniEmployment = await _context.AlumniEmployments.FindAsync(id);
            if (alumniEmployment == null)
            {
                return NotFound();
            }
            ViewData["ProgramId"] = new SelectList(_context.UniPrograms, "ProgramId", "ProgramId", alumniEmployment.ProgramId);
            return View(alumniEmployment);
        }

        // POST: AlumniEmployments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AlumniemploymentsEdit(int id, [Bind("Year,Gender,GradEmployed,GradEnrolled,AlumniId,ProgramId,NumOfStudentPassQiyasExam,NumOfStudentPassCareerExam,TotalGradEvalByCompany,NumOfCompanyEvaled")] AlumniEmployment alumniEmployment)
        {
            if (id != alumniEmployment.AlumniId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(alumniEmployment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlumniEmploymentExists(alumniEmployment.AlumniId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(AlumniEmploymentsIndex));
            }
            ViewData["ProgramId"] = new SelectList(_context.UniPrograms, "ProgramId", "ProgramId", alumniEmployment.ProgramId);
            return View(alumniEmployment);
        }

        

        // POST: AlumniEmployments/Delete/5
        [HttpPost]
        public async Task<IActionResult> deleteAlumniemp(int id)
        {
            var dep = await _context.AlumniEmployments.FindAsync(id);
            _context.AlumniEmployments.Remove(dep);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AlumniEmploymentsIndex));
        }




        private bool AlumniEmploymentExists(int id)
        {
            return _context.AlumniEmployments.Any(e => e.AlumniId == id);
        }
    }

}
