using NinjaDomain.Classes;
using NinjaDomain.DataModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            //used to stop db initialization
            Database.SetInitializer(new NullDatabaseInitializer<NinjaContext>());
            //InsertNinja();
            //InsertMulipleNinjas();
            //SimpleNinjaQueries();
            //QueryAndUpdateNinja();
            //RetriveDataWithFind();
            //RetriveDataWithStoredProc();
            //DeleteNinja();
            //DeleteSingleEntryWithStoredProc();
            //DeleteMultipleEntriesWithStoredProc();
            //DeleteMultipleEntriesWithStoredProc2();
            //InsertNinjaWithEquipment();
            SimpleNinjaGraphQuery();
            Console.ReadKey();


        }
        private static void InsertNinja()
        {
            //instantiate ninjacontext object
            var ninja = new Ninja
            {
                Name = "ObiEight",
                ServedInOniwaban = true,
                DateOfBirth = new DateTime(1944, 7, 2),
                ClanId = 1
            };

            //use EF to insert data to DB
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                // add lets to insert nly one
                context.Ninjas.Add(ninja);
                context.SaveChanges();
            }

        }

        private static void InsertMulipleNinjas()
        {
            //instantiate ninjacontext object
            var ninja3 = new Ninja
            {
                Name = "ObiThree",
                ServedInOniwaban = false,
                DateOfBirth = new DateTime(1938, 7, 22),
                ClanId = 1
            };
            var ninja4 = new Ninja
            {
                Name = "ObiFour",
                ServedInOniwaban = false,
                DateOfBirth = new DateTime(1942, 2, 8),
                ClanId = 1
            };

            //use EF to insert data to DB
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                context.Ninjas.AddRange(new List<Ninja> { ninja3, ninja4 });
                context.SaveChanges();
            }
        }
        // query data from db
        private static void SimpleNinjaQueries()
        {
            using (var context = new NinjaContext())
            {
                //var ninjas = context.Ninjas.Where(n => n.Name == "ObiOne");
                //var query = context.Ninjas;
                //ToList() is LINQ execution method
                //var someninjas = query.ToList();
                //foreach (var ninja in ninjas)
                //{
                //    Console.WriteLine(ninja.Name);
                //}
                // Where is LINQ method
                var ninja = context.Ninjas.Where(n => n.Name == "ObiOne").FirstOrDefault();
                Console.WriteLine(ninja.Name);
            }
        }

        private static void QueryAndUpdateNinja()
        {
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                var ninja = context.Ninjas.FirstOrDefault();
                ninja.ServedInOniwaban = (!ninja.ServedInOniwaban);
                context.SaveChanges();
            }
        }
        private static void RetriveDataWithFind()
        {
            using (var context = new NinjaContext())
            {
                // retrieves row with id of value 7
                // Find check if the object already exists in the mmemory and is tracked by the context
                // if yes it doesnt query db
                var keyval = 7;
                context.Database.Log = Console.WriteLine;
                var ninja = context.Ninjas.Find(keyval);
                Console.WriteLine(ninja.Name);
            }
        }

        private static void RetriveDataWithStoredProc()
        {
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                //SqlQuery is a method of DBSet
                // result db schema must match the schema of the type
                var ninjas = context.Ninjas.SqlQuery("exec GetOldNinjas");
                foreach (var ninja in ninjas)
                {
                    Console.WriteLine(ninja.Name);
                }
            }
        }

        private static void DeleteNinja()
        {
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                var ninja = context.Ninjas.FirstOrDefault();
                context.Ninjas.Remove(ninja);
                context.SaveChanges();
            }
        }

        private static void DeleteSingleEntryWithStoredProc()
        {
            using (var context = new NinjaContext())
            {
                var keyval = 6;
                context.Database.Log = Console.WriteLine;
                context.Database.ExecuteSqlCommand("exec DeleteSingleEntry {0}", keyval);
            }
        }

        private static void DeleteMultipleEntriesWithStoredProc()
        {
            using (var context = new NinjaContext())
            {
                var keyval = 9;
                context.Database.Log = Console.WriteLine;
                context.Database.ExecuteSqlCommand("exec DeleteMultipleEntries {0}", keyval);
            }
        }

        private static void DeleteMultipleEntriesWithStoredProc2()
        {
            using (var context = new NinjaContext())
            {
                
                context.Database.Log = Console.WriteLine;
                context.Database.ExecuteSqlCommand("exec DeleteMultipleEntries2");
            }
        }

        private static void InsertNinjaWithEquipment()
        {
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;

                var ninja = new Ninja
                {
                    Name = "ObiFive",
                    ServedInOniwaban = true,
                    DateOfBirth = new DateTime(1945, 1, 9),
                    ClanId = 1
                };

                var muscles = new NinjaEquipment
                {
                    Name = "Muscles",
                    Type = EquipmentType.Tool
                };

                var spunk = new NinjaEquipment
                {
                    Name = "Spunk",
                    Type = EquipmentType.Weapon
                };
                context.Ninjas.Add(ninja);
                // remeber to instantiate the equipment list before adding it in Ninja class and instantiate ninjas list in Clan class
                ninja.EquipmentOwned.Add(muscles);
                ninja.EquipmentOwned.Add(spunk);
                context.SaveChanges();
            }
        }

        private static void SimpleNinjaGraphQuery()
        {
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                //1. Eager Loading : to request related data, include a method of DBSet.Include() retrieves all related data from DB in one call in advance.Ninja plus Equipment joins ninja data and equipment data: context.Ninjas.Include(n => n.Equipment).FirstOrDefault(n => n.Name == "ObiFive");
                // one call to db
                //var ninja = context.Ninjas.Include(n => n.EquipmentOwned).FirstOrDefault(n => n.Name == "ObiFive");


                //Explicit Loading: request related data on the fly. first ask for ninja then when it is back with it in memory ask for related collection, another call to db  use Entry property to let the context know we want related data, Load() method triggers executes SELECT query immediately,
                //2 connections to db
                var ninja = context.Ninjas.FirstOrDefault(n => n.Name == "ObiFive");
                context.Entry(ninja).Collection(n => n.EquipmentOwned).Load();
                Console.WriteLine(ninja.Name);
                //3.Lazy Loading - related data loaded on demand, add virtual to List<NinjaEquipment> in Ninja class, property should be virtual
                // related data loading is delayed till you specify request for it.

            }
        }     
    }
}
