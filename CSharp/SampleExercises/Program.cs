using SampleExercises;
using SimpleDataManagement.Models;

var dataSourcesDirectory = Path.Combine(Environment.CurrentDirectory, "DataSources");
var personsFilePath = Path.Combine(dataSourcesDirectory, "Persons_20220824_00.json");
var organizationsFilePath = Path.Combine(dataSourcesDirectory, "Organizations_20220824_00.json");
var vehiclesFilePath = Path.Combine(dataSourcesDirectory, "Vehicles_20220824_00.json");
var addressesFilePath = Path.Combine(dataSourcesDirectory, "Addresses_20220824_00.json");

//Quick test to ensure that all files are where they should be :)
foreach (var path in new[] { personsFilePath, organizationsFilePath, vehiclesFilePath, addressesFilePath })
{
    if (!File.Exists(path))
        throw new FileNotFoundException(path);
}

//TODO: Start your exercise here. Do not forget about answering Test #9 (Handled slightly different)
// Reminder: Collect the data from each file. Hint: You could use Newtonsoft's JsonConvert or Microsoft's JsonSerializer
var people = await JsonFileReader.ReadAsync<ICollection<Person>>(personsFilePath);
var organizations = await JsonFileReader.ReadAsync<ICollection<Organization>>(organizationsFilePath);
var vehicles = await JsonFileReader.ReadAsync<ICollection<Vehicle>>(vehiclesFilePath);
var addresses = await JsonFileReader.ReadAsync<ICollection<Address>>(addressesFilePath);

//Test #1: Do all files have entities? (True / False)
Console.WriteLine("#1 Do all files have data?");
var doAllFilesHaveData = people.Any() && organizations.Any() && vehicles.Any() && addresses.Any();
Console.WriteLine(doAllFilesHaveData ? "true" : "false");
NewLine();

//Test #2: What is the total count for all entities?
Console.WriteLine("#2 What is the total count for all entities?");
Console.WriteLine(people.Count + organizations.Count + vehicles.Count + addresses.Count);
NewLine();

//Test #3: What is the count for each type of Entity? Person, Organization, Vehicle, and Address
Console.WriteLine("#3 What is the count for each type of Entity? Person, Organization, Vehicle, and Address");
Console.WriteLine("Person: " + people.Count);
Console.WriteLine("Organization: " + organizations.Count);
Console.WriteLine("Vehicle: " + vehicles.Count);
Console.WriteLine("Address: " + addresses.Count);
NewLine();

//Test #4: Provide a breakdown of entities which have associations in the following manor:
//         -Per Entity Count
//         - Total Count
Console.WriteLine("#4 Provide a breakdown of entities which have associations in the prescribed manor");
var totalPeopleWithAssociations = people.Count(x => x.Associations.Any());
var totalOrganizationsWithAssociations = organizations.Count(x => x.Associations.Any());
var totalVehiclesWithAssociations = vehicles.Count(x => x.Associations.Any());
var totalAddressesWithAssociations = addresses.Count(x => x.Associations.Any());
Console.WriteLine("Person: " + totalPeopleWithAssociations);
Console.WriteLine("Organization: " + totalOrganizationsWithAssociations);
Console.WriteLine("Vehicle: " + totalVehiclesWithAssociations);
Console.WriteLine("Address: " + totalAddressesWithAssociations);
Console.WriteLine("Total: " + (totalPeopleWithAssociations + totalOrganizationsWithAssociations + totalVehiclesWithAssociations + totalAddressesWithAssociations));
NewLine();

//Test #5: Provide the vehicle detail that is associated to the address "4976 Penelope Via South Franztown, NH 71024"?
Console.WriteLine("#5 Provide the vehicle detail that is associated to the address \"4976 Penelope Via South Franztown, NH 71024\"?");
var penelope = addresses.FirstOrDefault(x => string.Equals(FullyQualifiedAddress(x), "4976 Penelope Via South Franztown, NH 71024", StringComparison.OrdinalIgnoreCase));
//In reality, we'd probably need to account for the fact that an address could have multiple vehicles
var penelopeVehicleAssociation = penelope!.Associations.FirstOrDefault(x => x.EntityType == "Vehicle");
var penelopesVehicle = vehicles.FirstOrDefault(x => x.EntityId == penelopeVehicleAssociation!.EntityId);
Console.WriteLine("Id: " + penelopesVehicle!.EntityId);
Console.WriteLine("Make: " + penelopesVehicle.Make);
Console.WriteLine("Model: " + penelopesVehicle.Model);
Console.WriteLine("Year: " + penelopesVehicle.Year);
Console.WriteLine("Plate: " + penelopesVehicle.PlateNumber);
Console.WriteLine("State: " + penelopesVehicle.State);
Console.WriteLine("Vin: " + penelopesVehicle.Vin);
NewLine();

//Test #6: What person(s) are associated to the organization "Thiel and Sons"?
Console.WriteLine("#6 What person(s) are associated to the organization \"Thiel and Sons\"?");
var thielAndSons = organizations.FirstOrDefault(x => string.Equals(x.Name, "Thiel and Sons", StringComparison.Ordinal));
var peopleThatWorkAtThielAndSons = people
    .SelectMany(person => person.Associations, (person, association) => new { person, association })
    .Where(@t => @t.association.EntityType == "Organization" && @t.association.EntityId == thielAndSons!.EntityId)
    .Select(@t => @t.person);
if (!peopleThatWorkAtThielAndSons.Any())
    Console.WriteLine("None");
else
{
    //We would foreach list out the people here if there were any
}
NewLine();

//Test #7: How many people have the same first and middle name?
Console.WriteLine("#7 How many people have the same first and middle name?");
var peopleWithSameFirstAndMiddleNames = people
    .Where(x => string.Equals(x.FirstName, x.MiddleName, StringComparison.OrdinalIgnoreCase));
Console.WriteLine(peopleWithSameFirstAndMiddleNames.Count());
NewLine();

//Test #8: Provide a breakdown of entities where the EntityId contains "B3" in the following manor:
//         -Total count by type of Entity
//         - Total count of all entities
Console.WriteLine("#8 Provide a breakdown of entities where the EntityId contains \"B3\" in the provided manor");
var totalPeopleWhereEntityIdContainsB3 = people.Count(x => x.EntityId.ToUpper().Contains("B3"));
var totalOrganizationsWhereEntityIdContainsB3 = organizations.Count(x => x.EntityId.ToUpper().Contains("B3"));
var totalVehiclesWhereEntityIdContainsB3 = vehicles.Count(x => x.EntityId.ToUpper().Contains("B3"));
var totalAddressesWhereEntityIdContainsB3 = addresses.Count(x => x.EntityId.ToUpper().Contains("B3"));
Console.WriteLine("Person: " + totalPeopleWhereEntityIdContainsB3);
Console.WriteLine("Vehicle: " + totalVehiclesWhereEntityIdContainsB3);
Console.WriteLine("Organization: " + totalOrganizationsWhereEntityIdContainsB3);
Console.WriteLine("Address: " + totalAddressesWhereEntityIdContainsB3);
Console.WriteLine("Total: " + (totalPeopleWhereEntityIdContainsB3 + totalOrganizationsWhereEntityIdContainsB3
                                                                  + totalVehiclesWhereEntityIdContainsB3
                                                                  + totalAddressesWhereEntityIdContainsB3));
NewLine();

string FullyQualifiedAddress(Address address)
{
    return address.StreetAddress + " " + address.City + ", " + address.State + " " + address.ZipCode;
}

void NewLine()
{
    Console.WriteLine();
}

//#9 What improvements would you make to the object model to improve your overall workflow?
/*
 *Instead of having the Associations be general and identified with a type I would probably have 
 *a collection for each entity similar to foreign key relationships in a relational database (Address would
 * have a list of Vehicles for example, more explicit). I would also *change the c# POCOs to use GUID
 * instead of string and I would rename the "EntityId" field to "Id".
 */