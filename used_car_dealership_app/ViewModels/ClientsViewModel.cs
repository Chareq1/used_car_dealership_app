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
