using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.Data;
using System;

namespace P01_StudentSystem
{
    public  class StartUp
    {
        static void Main(string[] args)
        { 
            StudentSystemContext db = new StudentSystemContext();
            db.Database.Migrate();
            Console.WriteLine($"Database created successfully!");
        }
    }
}
