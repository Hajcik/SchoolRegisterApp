using System.Collections.Generic;

namespace SchoolRegister.BLL.Entities
{
    public class Parent : User
    {
        public virtual IList<Student> Students { get; set; }
    }
}