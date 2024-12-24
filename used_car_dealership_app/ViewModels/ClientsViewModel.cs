using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using used_car_dealership_app.Models;
using used_car_dealership_app.Repositories;
using used_car_dealership_app.Database;

namespace used_car_dealership_app.ViewModels;

public class ClientsViewModel : ViewModelBase
{
    private readonly CustomerRepository customerRepository;
    private ObservableCollection<Customer> customers;

    public ClientsViewModel()
    {
        customerRepository = new CustomerRepository();
        LoadCustomers(); // Load customers initially
    }

    public ObservableCollection<Customer> Customers
    {
        get => customers;
        set => SetProperty(ref customers, value);
    }

    private async Task LoadCustomers()
    {
        // Fetch customers from the repository asynchronously
        var dataTable = await Task.Run(() => customerRepository.GetAllCustomers());
        
        dataTable.Rows.Add(Guid.NewGuid(), "John Doe I", "john.doe@example.com", "1234567890");
        dataTable.Rows.Add(Guid.NewGuid(), "Jane Smith I", "jane.smith@example.com", "0987654321");
        dataTable.Rows.Add(Guid.NewGuid(), "Alice Johnson I", "alice.johnson@example.com", "5678901234");
        dataTable.Rows.Add(Guid.NewGuid(), "Bob Brown I", "bob.brown@example.com", "2345678901");
        dataTable.Rows.Add(Guid.NewGuid(), "John Doe II", "john.doe@example.com", "1234567890");
        dataTable.Rows.Add(Guid.NewGuid(), "Jane Smith II", "jane.smith@example.com", "0987654321");
        dataTable.Rows.Add(Guid.NewGuid(), "Alice Johnson II", "alice.johnson@example.com", "5678901234");
        dataTable.Rows.Add(Guid.NewGuid(), "Bob Brown II", "bob.brown@example.com", "2345678901");

        // Convert DataTable to List<Customer>
        var customers = dataTable.AsEnumerable().Select(row => new Customer
        {
            CustomerId = Guid.Parse(row["customerId"].ToString()),
            Name = row["name"].ToString(),
            Email = row["email"].ToString(),
            Phone = row["phone"].ToString()
        }).ToList();
        
        

        // Create an ObservableCollection from the list
        Customers = new ObservableCollection<Customer>(customers);
    }
}
