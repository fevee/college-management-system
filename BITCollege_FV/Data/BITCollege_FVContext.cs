using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BITCollege_FV.Data
{
    public class BITCollege_FVContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public BITCollege_FVContext() : base("name=BITCollege_FVContext")
        {
        }

        public System.Data.Entity.DbSet<BITCollege_FV.Models.AcademicProgram> AcademicPrograms { get; set; }

        public System.Data.Entity.DbSet<BITCollege_FV.Models.Student> Students { get; set; }

        public System.Data.Entity.DbSet<BITCollege_FV.Models.GradePointState> GradePointStates { get; set; }

        public System.Data.Entity.DbSet<BITCollege_FV.Models.RegularState> RegularStates { get; set; }

        public System.Data.Entity.DbSet<BITCollege_FV.Models.HonoursState> HonoursStates { get; set; }
        
        public System.Data.Entity.DbSet<BITCollege_FV.Models.SuspendedState> SuspendedStates { get; set; }
        
        public System.Data.Entity.DbSet<BITCollege_FV.Models.ProbationState> ProbationStates { get; set; }

        public System.Data.Entity.DbSet<BITCollege_FV.Models.Registration> Registrations { get; set; }

        public System.Data.Entity.DbSet<BITCollege_FV.Models.Course> Courses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_FV.Models.GradedCourse> GradedCourses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_FV.Models.AuditCourse> AuditCourses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_FV.Models.MasteryCourse> MasteryCourses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_FV.Models.NextStudent> NextStudents { get; set; }

        public System.Data.Entity.DbSet<BITCollege_FV.Models.NextRegistration> NextRegistrations { get; set; }

        public System.Data.Entity.DbSet<BITCollege_FV.Models.NextGradedCourse> NextGradedCourses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_FV.Models.NextAuditCourse> NextAuditCourses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_FV.Models.NextMasteryCourse> NextMasteryCourses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_FV.Models.NextUniqueNumber> NextUniqueNumbers { get; set; }
    }
}
