using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {
        static HumaneSocietyDataContext db;

        static Query()
        {
            db = new HumaneSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();

            return allStates;
        }

        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = null;

            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("No clients have a ClientId that matches the Client passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }

            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;

            // submit changes
            db.SubmitChanges();
        }
        
        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).First();
            //**FIXED THIS CODE. WAS LETTING ANYTHING GO THROUGH**//
            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();
            if (employeeWithUserName == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        //// TODO Items: ////

        // TODO: Allow any of the CRUD operations to occur here
        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {
            switch (crudOperation)
            {
                case "create":
                    AddEmployee(employee);
                    break;
                case "read":
                    GetEmployee(employee);
                    break;
                case "update":
                    UpdateEmployee(employee);
                    break;
                case "delete":
                    DeleteEmployee(employee);
                    break;
                default:
                    break;
            }
        }
        internal static void AddEmployee(Employee employee)
        {
            db.Employees.InsertOnSubmit(employee);
            db.SubmitChanges();
        }
        internal static void UpdateEmployee(Employee employee)
        {
            var newEmployee = db.Employees.Where(e => e.FirstName == employee.FirstName && e.LastName == employee.LastName && e.EmployeeNumber == employee.EmployeeNumber && e.Email == employee.Email).Select(e => e).SingleOrDefault();

            if (newEmployee == null)
            {
                Console.WriteLine("No employee found to update.");
                Console.ReadLine();
            }
            else
            {
                newEmployee.FirstName = UserInterface.GetStringData("first name", "the employee's");
                newEmployee.LastName = UserInterface.GetStringData("last name", "the employee's");
                newEmployee.EmployeeNumber = int.Parse(UserInterface.GetStringData("employee number", "the employee's"));
                newEmployee.Email = UserInterface.GetStringData("email", "the employee's"); ;
                newEmployee.FirstName = UserInterface.GetStringData("username", "the employee's");
                newEmployee.LastName = UserInterface.GetStringData("password", "the employee's");
                db.SubmitChanges();
            }
        }
        internal static void DeleteEmployee(Employee employee)
        {
            var oldEmployee = db.Employees.Where(e => e.LastName == employee.LastName && e.EmployeeNumber == employee.EmployeeNumber).Select(e => e).SingleOrDefault();
            if (oldEmployee == null)
            {
                Console.WriteLine("No employees found.");
                Console.ReadLine();
            }
            else
            {
                db.Employees.DeleteOnSubmit(oldEmployee);
                db.SubmitChanges();
                Console.WriteLine("Delete successful.");
                Console.ReadLine();
            }
        }
        internal static void GetEmployee(Employee employee)
        {
            var grunt = db.Employees.Where(e => e.EmployeeNumber == employee.EmployeeNumber).Select(e => e).Single();
            Console.WriteLine("First Name: " + grunt.FirstName + "\n" + "Last Name: " + grunt.LastName + "\n" + "Username: " + grunt.UserName + "\n" + "Password: " + grunt.Password + "\n" + "Email: " + grunt.Email + "\n" + "Employee Number: " + grunt.EmployeeNumber);
            Console.ReadLine();
        }
        internal static void AddAnimal(Animal animal)
        {
            db.Animals.InsertOnSubmit(animal);
            AddToRoom(animal);

            
            db.SubmitChanges();
        }

        internal static void AddToRoom(Animal animal)
        {
            Room newRoom = null;
            newRoom.AnimalId = animal.AnimalId;
            newRoom.Animal = animal;
            db.Rooms.InsertOnSubmit(newRoom);
            db.SubmitChanges();
        }

        internal static void RemoveFromRoom(Animal animal)
        {
            var room = db.Rooms.Where(r => r.AnimalId == animal.AnimalId).Select(r => r).Single();
            db.Rooms.DeleteOnSubmit(room);
            db.SubmitChanges();
        }

        internal static Animal GetAnimalByID(int id)
        {
            return db.Animals.Where(a => a.AnimalId == id).Select(a => a).Single();
        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {
            Animal animal = db.Animals.Where(a => a.AnimalId == animalId).Select(a => a).Single();
            if (updates[1] != null)
            {
                animal.Category.Name = updates[1];
            }
            if (updates[2] != null)
            {
                animal.Name = updates[2];
            }
            if (updates[3] != null)
            {
                animal.Age = Convert.ToInt32(updates[3]);
            }
            if (updates[4] != null)
            {
                animal.Demeanor = updates[4];
            }
            if (updates[5] != null)
            {
                animal.KidFriendly = Convert.ToBoolean(updates[5]);
            }
            if (updates[6] != null)
            {
                animal.PetFriendly = Convert.ToBoolean(updates[6]);
            }
            if (updates[7] != null)
            {
                animal.Weight = Convert.ToInt32(updates[7]);
            }
            db.Animals.InsertOnSubmit(animal);
            db.SubmitChanges();
        }

        internal static void RemoveAnimal(Animal animal)
        {
            db.Animals.DeleteOnSubmit(animal);
            RemoveFromRoom(animal);
            var clientid = db.Adoptions.Where(a => a.AnimalId == animal.AnimalId).Select(a => a.ClientId).Single();
            RemoveAdoption(animal.AnimalId, clientid);
            db.SubmitChanges();
        }

        // TODO: Animal Multi-Trait Search
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            var animalList = db.Animals.Select(a => a);
            foreach (var item in updates)
            {
                switch (item.Key)
                {
                    case 1:
                        animalList = animalList.Where(a => a.Category.Name == updates[item.Key]).Select(a => a);
                        break;
                    case 2:
                        animalList = animalList.Where(a => a.Name == updates[item.Key]).Select(a => a);
                        break;
                    case 3:
                        animalList = animalList.Where(a => a.Age == Convert.ToInt32(updates[item.Key])).Select(a => a);
                        break;
                    case 4:
                        animalList = animalList.Where(a => a.Demeanor == updates[item.Key]).Select(a => a);
                        break;
                    case 5:
                        animalList = animalList.Where(a => a.KidFriendly == Convert.ToBoolean(updates[item.Key])).Select(a => a);
                        break;
                    case 6:
                        animalList = animalList.Where(a => a.PetFriendly == Convert.ToBoolean(updates[item.Key])).Select(a => a);
                        break;
                    case 7:
                        animalList = animalList.Where(a => a.Weight == Convert.ToInt32(updates[item.Key])).Select(a => a);
                        break;
                    case 8:
                        animalList = animalList.Where(a => a.AnimalId == Convert.ToInt32(updates[item.Key])).Select(a => a);
                        break;
                    default:
                        break;
                }
            }
            return animalList;
        }

        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName)
        {
            return db.Categories.Where(c => c.Name == categoryName).Select(c => c.CategoryId).Single();
        }

        internal static Room GetRoom(int animalId)
        {
            return db.Rooms.Where(r => r.AnimalId == animalId).Select(r => r).Single();
        }

        internal static int GetDietPlanId(string dietPlanName)
        {
            return db.DietPlans.Where(dp => dp.Name == dietPlanName).Select(dp => dp.DietPlanId).Single();
        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {
            Adoption adoption = null;
            adoption.AnimalId = animal.AnimalId;
            adoption.ClientId = client.ClientId;
            adoption.ApprovalStatus = "Pending";
            adoption.AdoptionFee = 75;
            adoption.PaymentCollected = false;
            db.Adoptions.InsertOnSubmit(adoption);
            db.SubmitChanges();
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            return db.Adoptions.Where(a => a.ApprovalStatus == "Pending").Select(a => a);
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            if (isAdopted == true)
            {
                var newAdopt = db.Adoptions.Where(a => a.AnimalId == adoption.AnimalId && a.ClientId == adoption.ClientId).Select(a => a).Single();
                newAdopt.ApprovalStatus = "Approved";
                newAdopt.PaymentCollected = true;
                db.SubmitChanges();
            }
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            var adopt = db.Adoptions.Where(adoption => adoption.ClientId == clientId && adoption.AnimalId == animalId).Select(adoption => adoption).SingleOrDefault();
            if (adopt != null)
            {
                db.Adoptions.DeleteOnSubmit(adopt);
                db.SubmitChanges();
            }
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            return db.AnimalShots.Where(s => s.AnimalId == animal.AnimalId).Select(s => s);
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            DateTime dateTime = new DateTime();
            AnimalShot animalShot = db.AnimalShots.Where(s => s.Shot.Name == shotName && s.AnimalId == animal.AnimalId).Select(s => s).SingleOrDefault();
            if (animalShot == null) 
            {
                Shot shot = new Shot();
                shot.Name = shotName;
                db.Shots.InsertOnSubmit(shot);
                db.SubmitChanges();
                animalShot.AnimalId = animal.AnimalId;
                animalShot.ShotId = shot.ShotId;
                animalShot.DateReceived = dateTime.Date;
                db.AnimalShots.InsertOnSubmit(animalShot);
                db.SubmitChanges();
            }
            else
            {
                animalShot.DateReceived = dateTime.Date;
                db.SubmitChanges();
            }
        }
    }
}