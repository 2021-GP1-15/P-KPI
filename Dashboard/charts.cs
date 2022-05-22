using IP_KPI.Data;
using IP_KPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IP_KPI.Dashboard
{
    public class charts
    {
        private readonly db_a7baa5_ipkpiContext _db;

        public charts(db_a7baa5_ipkpiContext db)
        {
            _db = db;
        }

        public String progName(String programId)
        {
            var program = _db.UniPrograms.FirstOrDefault(x => x.ProgramId == Convert.ToInt32(programId));
            return program.Level + " " + program.ProgramName;
        }
        public float percentage(int count, float num1, float num2, float num3, float num4, float num5, float num6)
        {
            if (count == 6)
                return (num1 / (num1 + num2 + num3 + num4 + num5 + num6)) * 100;
            if (count == 2)
                return (num1 / (num1 + num2)) * 100;
            return 0;
        }
        public String color(int count)
        {
            switch (count)
            {
                case 0:
                    return "#D0DA32";
                case 1:
                    return "#1F3771";
                case 2:
                    return "#71BA44";
                case 3:
                    return "#29B8BE";
            }
            return "#B0AEB3";
        }

        //survey KPIs
        public float serveyCount(List<Survey> survey, char type, String val, String val2)
        {
            float count = 0;
            switch (type)
            {
                case 't'://term
                    foreach (var item in survey)
                    {
                        if (item.Term.Equals(val))
                            count += (float)item.NumOfRespondent;
                    }
                    return count;
                case 's'://student case
                    foreach (var item in survey)
                    {
                        if (item.StudentCase.Equals(val))
                            count += (float)item.NumOfRespondent;
                    }
                    return count;
                case 'g'://gender
                    foreach (var item in survey)
                    {
                        if (item.Gender.Equals(val))
                            count += (float)item.NumOfRespondent;
                    }
                    return count;
                case 'n'://nationality
                    foreach (var item in survey)
                    {
                        if (item.Nationality.Equals(val))
                            count += (float)item.NumOfRespondent;
                    }
                    return count;
                case 'c'://count of student
                    foreach (var item in survey)
                        count += (float)item.NumberOfStudent;
                    return count;
                case '2'://count of student
                    foreach (var item in survey)
                        if (item.Gender.Equals(val))
                        {
                            if (item.Term.Equals(val2))
                                count += (float)item.NumberOfStudent;
                        }
                    return count;
                case 'f'://four value
                    foreach (var item in survey)
                    {
                        count += (float)item.NumOfRespondent;
                    }
                    return count;
                default:
                    {
                        return count;
                    }
            }
        }
        public float serveyScore(List<Survey> survey, char type, String val)
        {
            float score = 0;
            switch (type)
            {
                case 't'://term
                    foreach (var item in survey)
                    {
                        if (item.Term.Equals(val))
                            score += (float)item.SurveyScore;
                    }
                    return score;
                case 's'://student case
                    foreach (var item in survey)
                    {
                        if (item.StudentCase.Equals(val))
                            score += (float)item.SurveyScore;
                    }
                    return score;
                case 'g'://gender
                    foreach (var item in survey)
                    {
                        if (item.Gender.Equals(val))
                            score += (float)item.SurveyScore;
                    }
                    return score;
                case 'n'://nationality
                    foreach (var item in survey)
                    {
                        if (item.Nationality.Equals(val))
                            score += (float)item.SurveyScore;
                    }
                    return score;
                case 'f'://four value
                    foreach (var item in survey)
                    {
                        score += (float)item.SurveyScore;
                    }
                    return score;
                default:
                    return 0;
            }
        }
        public List<Survey> filterList(List<Survey> survey, String nationality, String gender, String studentCase, String term)
        {
            if (nationality != null && nationality != "0")
                survey = survey.Where(x => x.Nationality == nationality).ToList();

            if (gender != null && gender != "0")
                survey = survey.Where(x => x.Gender == gender).ToList();

            if (studentCase != null && studentCase != "0")
                survey = survey.Where(x => x.StudentCase == studentCase).ToList();

            if (term != null && term != "0")
                survey = survey.Where(x => x.Term == term).ToList();

            return survey;
        }

        //Alumni KPIs
        public float alumniResult(List<AlumniEmployment> alumniEmployments, String gender, char choice)
        {
            if (gender != null && gender != "0")
                alumniEmployments = alumniEmployments.Where(x => x.Gender == gender).ToList();

            float count = 0, score = 0; 
            switch (choice)
            {
                case 'e'://grad Employed
                    {
                        foreach (var item in alumniEmployments)
                            count += (float)item.GradEmployed;
                        break;
                    }
                case 'm'://grad continue to Master
                    {
                        foreach (var item in alumniEmployments)
                            count += (float)item.GradEnrolled;
                        break;
                    }
                case 'q'://grad passed Qiyas Exam
                    {
                        foreach (var item in alumniEmployments)
                            count += (float)item.NumOfStudentPassQiyasExam;
                        break;
                    }
                case 'r'://grad passed Career Exam
                    {
                        foreach (var item in alumniEmployments)
                            count += (float)item.NumOfStudentPassCareerExam;
                        break;
                    }
                case 'c'://avg Company evaluation
                    {
                        foreach (var item in alumniEmployments)
                        {
                            score += (float)item.TotalGradEvalByCompany;
                            count += (float)item.NumOfCompanyEvaled;
                        }
                        count = score / count;
                        break;
                    }
            }
            return count;
        }


        //KPIProgram table
        public float KPIscore(List<Kpiprogram> kpiProg, char type)
        {
            float score = 0;
            switch (type)
            {
                case 'i'://internal
                    foreach (var item in kpiProg)
                        score += (float)item.InternalBenchmark;
                    return score / kpiProg.Count;
                case 'e'://external
                    foreach (var item in kpiProg)
                        score += (float)item.ExternalBenchmark;
                    return score / kpiProg.Count;
                case 'k'://KPI target
                    foreach (var item in kpiProg)
                        score += (float)item.TargetBenchmark;
                    return score / kpiProg.Count;
                default:
                    return 0;
            }
        }
        public List<Kpiprogram> filterKPIList(List<Kpiprogram> Kpiprogram, String term, String gender)
        {
            if (term != null && term != "0")
                Kpiprogram = Kpiprogram.Where(x => x.Term == term).ToList();

            if (gender != null && gender != "0")
                Kpiprogram = Kpiprogram.Where(x => x.Gender == gender).ToList();

            return Kpiprogram;
        }

        //Publication KPIs
        public float PublicationScore(List<PublicationReport> PublicationReports, List<FacultyStatistic> facultyStatistics, char type, String gender)
        {
            if (gender != null && gender != "0")
            {
                PublicationReports = PublicationReports.Where(x => x.Gender == gender).ToList();
            }
            float score = 0;
            switch (type)
            {
                case 'f'://faculty     
                    return numOfFaculty(facultyStatistics, gender, null, 'a'); ;
                case 'p'://publication
                    foreach (var item in PublicationReports)
                        score += (float)item.NumOfPublications;
                    return score;
                case 'c'://citation
                    foreach (var item in PublicationReports)
                        score += (float)item.NumOfCitations;
                    return score;
                case 'o'://faculty with at least one publication
                    foreach (var item in PublicationReports)
                        score += (float)item.NumOfFacultyOneP;
                    return score;
                default:
                    return 0;
            }

        }

        //Faculty KPIs
        public float numOfFaculty(List<FacultyStatistic> facultyStatistics, String gender, String type, char choice)
        {

            if (gender != null && gender != "0")
                facultyStatistics = facultyStatistics.Where(x => x.Gender == gender).ToList();

            if (type != null && type != "0")
                facultyStatistics = facultyStatistics.Where(x => x.Type == type).ToList();

            float count = 0;
            switch (choice)
            {
                case 'a'://all
                    {
                        foreach (var item in facultyStatistics)
                            count += (float)(item.NumOfAssistantProf + item.NumOfAssociateProf + item.NumOfLecturer + item.NumOfProf + item.NumOfTeacher + item.NumOfTeachingAssistant);
                        return count;
                    }
                case 'b'://AssistantPorf
                    {
                        foreach (var item in facultyStatistics)
                            count += (float)item.NumOfAssistantProf;
                        return count;
                    }
                case 'c'://AssociateProf
                    {
                        foreach (var item in facultyStatistics)
                            count += (float)item.NumOfAssociateProf;
                        return count;
                    }
                case 'd'://Lecturer
                    {
                        foreach (var item in facultyStatistics)
                            count += (float)item.NumOfLecturer;
                        return count;
                    }
                case 'e'://Prof
                    {
                        foreach (var item in facultyStatistics)
                            count += (float)item.NumOfProf;
                        return count;
                    }
                case 'f'://Teacher
                    {
                        foreach (var item in facultyStatistics)
                            count += (float)item.NumOfTeacher;
                        return count;
                    }
                case 'g'://TeachingAssistant
                    {
                        foreach (var item in facultyStatistics)
                            count += (float)item.NumOfTeachingAssistant;
                        return count;
                    }
                case 'r':
                    {
                        foreach (var item in facultyStatistics)
                            count += (float)item.NumOfResignation;
                        return count;
                    }
            }
            return count;
        }

        //ClassSection KPI
        public float numOfclasses(List<ClassSection> classSections, String gender, String term)
        {
            //filter the list
            if (gender != null && gender != "0")
                classSections = classSections.Where(x => x.Gender == gender).ToList();

            if (term != null && term != "0")
                classSections = classSections.Where(x => x.Term == term).ToList();

            float count = 0;
            foreach (var item in classSections)
                count += (float)item.NumOfClasses;
            return count;
        }

    }


}


