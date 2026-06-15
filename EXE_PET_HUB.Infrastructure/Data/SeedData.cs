using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EXE_PET_HUB.Domain.Entities;
using EXE_PET_HUB.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace EXE_PET_HUB.Infrastructure.Data
{
    public class SeedData
    {
        private static readonly string DefaultStoreId = "44444444-4444-4444-4444-444444444444";
        public async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<AppDbContext>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            try { 
                //for identity
                await SeedRolesAsync(roleManager);
                await SeedUsersAsync(userManager);
                await SeedStoresAsync(context);
                await SeedStoreCustomersAsync(context);
                await SeedItemsAsync(context);
                Console.WriteLine("Done Identity, Store & Items");

                //for service
                await SeedPetsAsync(context);
                Console.WriteLine("Done Pets");
                await SeedAppointmentsAsync(context);
                await SeedAppointmentRemindersAsync(context);
                await SeedMedicalRecordsAsync(context);
                Console.WriteLine("Done Medical Process");

                //for payment
                await SeedInvoicesAsync(context);
                await SeedStorePackagePaymentsAsync(context);
                Console.WriteLine("All Seed Completed!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error: {ex.Message}");
                throw; 
            }
        }

        // 1. Seed Roles
        private static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
        {
            var roles = new[] { "admin", "manager", "customer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid> { Name = role, NormalizedName = role.ToUpper() });
                }
            }
        }

        // 2. Seed Users
        private static async Task SeedUsersAsync(UserManager<User> userManager)
        {
            if (userManager.Users.Any()) return;

            await CreateUserAsync(userManager, "admin", "nguyenhuy3112005@gmail.com", "Admin@123", "Admin", "User", "admin");
            await CreateUserAsync(userManager, "manager", "kietdtse183938@fpt.edu.vn", "Manager@123", "Manager", "User", "manager");
            await CreateUserAsync(userManager, "customer", "huyndse184016@fpt.edu.vn", "Customer@123", "Customer", "User", "customer");
        }

        private static async Task CreateUserAsync(UserManager<User> userManager, string username, string email, string password, string firstName, string lastName, string role)
        {
            if (await userManager.FindByNameAsync(username) == null)
            {
                var user = new User
                {
                    UserName = username,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }

        // 3. Seed Stores
        private static async Task SeedStoresAsync(AppDbContext context)
        {
            if (context.Stores.Any()) return;

            var adminUser = context.Users.FirstOrDefault(u => u.UserName == "admin");

            if (adminUser != null)
            {
                var systemStore = new Store
                {
                    Id = DefaultStoreId,
                    ManagerId = adminUser.Id,
                    Name = "Hệ thống Quản lý Pet Hub",
                    Address = "Hệ thống",
                    Phone = "0000000000",
                    storeImage = "",
                    IsActive = true,
                    CreateAt = DateTime.UtcNow.AddHours(7),
                    UpdateAt = DateTime.UtcNow.AddHours(7)
                };

                context.Stores.Add(systemStore);
                await context.SaveChangesAsync();
            }

            var managers = context.Users
                .Where(u => u.UserName != null && u.UserName.Contains("manager"))
                .ToList();

            if (!managers.Any()) return;

            var stores = new List<Store>
            {
                new Store
                {
                    Id = Guid.NewGuid().ToString(),
                    ManagerId = managers[0].Id,
                    Name = "Pet Hub Quận 1",
                    Address = "123 Nguyễn Huệ, Quận 1, TP.HCM",
                    Phone = "0901234567",
                    storeImage = "",
                    IsActive = true,
                    CreateAt = DateTime.UtcNow.AddHours(7),
                    UpdateAt = DateTime.UtcNow.AddHours(7)
                },
                new Store
                {
                    Id = Guid.NewGuid().ToString(),
                    // Nếu cậu có nhiều manager, có thể đổi thành managers[1].Id cho đa dạng nhé
                    ManagerId = managers.Count > 1 ? managers[1].Id : managers[0].Id,
                    Name = "Pet Hub Bình Thạnh",
                    Address = "456 Điện Biên Phủ, Phường 25, Quận Bình Thạnh, TP.HCM",
                    Phone = "0909876543",
                    storeImage = "",
                    IsActive = true,
                    CreateAt = DateTime.UtcNow.AddHours(7),
                    UpdateAt = DateTime.UtcNow.AddHours(7)
                }
            };

            context.Stores.AddRange(stores);
            await context.SaveChangesAsync();
        }

        // 4. Seed Store Customers
        private static async Task SeedStoreCustomersAsync(AppDbContext context)
        {
            if (context.StoreCustomers.Any()) return;

            // LẤY TẤT CẢ các cửa hàng thực tế (Bỏ qua DefaultStoreId)
            var realStores = context.Stores.Where(s => s.Id != DefaultStoreId).ToList();
            if (!realStores.Any()) return;

            var customerRoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");
            var customerIds = context.UserRoles
                .Where(ur => ur.RoleId == customerRoleId)
                .Select(ur => ur.UserId)
                .ToList();

            if (!customerIds.Any()) return;

            var random = new Random();
            var storeCustomers = customerIds.Select(customerId => new StoreCustomer
            {
                Id = Guid.NewGuid().ToString(),
                // RANDOM ngẫu nhiên một store thực tế cho khách hàng
                StoreId = realStores[random.Next(realStores.Count)].Id,
                CustomerId = customerId,
                CreateAt = DateTime.UtcNow.AddHours(7)
            }).ToList();

            context.StoreCustomers.AddRange(storeCustomers);
            await context.SaveChangesAsync();
        }

        // 5. Seed Pets
        private static async Task SeedPetsAsync(AppDbContext context)
        {
            if (context.Pets.Any()) return;

            // Lấy danh sách StoreCustomer của các store thực tế
            var storeCustomers = context.StoreCustomers
                .Where(sc => sc.StoreId != DefaultStoreId)
                .ToList();

            if (!storeCustomers.Any()) return;

            var random = new Random();
            var petData = new[]
            {
                new { Name = "LuLu", Species = "Chó Poodle", Color = "Nâu", DateOfBirth = new DateOnly(2022, 5, 10) },
                new { Name = "Mimi", Species = "Mèo Anh lông ngắn", Color = "Xám xanh", DateOfBirth = new DateOnly(2023, 1, 15) },
                new { Name = "Ngáo", Species = "Chó Husky", Color = "Đen trắng", DateOfBirth = new DateOnly(2021, 11, 20) },
                new { Name = "Bánh Bao", Species = "Mèo Ba Tư", Color = "Trắng", DateOfBirth = new DateOnly(2023, 3, 5) },
                new { Name = "Xúc Xích", Species = "Chó Dachshund", Color = "Đen vàng", DateOfBirth = new DateOnly(2022, 8, 12) },
                new { Name = "Kem", Species = "Chó Samoyed", Color = "Trắng tuyết", DateOfBirth = new DateOnly(2022, 12, 25) },
                new { Name = "Mướp", Species = "Mèo Ta", Color = "Vằn", DateOfBirth = new DateOnly(2020, 6, 30) },
                new { Name = "Bơ", Species = "Chó Golden Retriever", Color = "Vàng kim", DateOfBirth = new DateOnly(2021, 4, 18) },
                new { Name = "Đậu Đậu", Species = "Chó Corgi", Color = "Cam trắng", DateOfBirth = new DateOnly(2023, 2, 14) },
                new { Name = "Mun", Species = "Mèo Munchkin", Color = "Tam thể", DateOfBirth = new DateOnly(2023, 5, 20) }
            };

            var pets = petData.Select(data =>
            {
                // Bốc ngẫu nhiên 1 cặp khách hàng - cửa hàng thực tế
                var storeCustomer = storeCustomers[random.Next(storeCustomers.Count)];
                return new Pet
                {
                    Id = Guid.NewGuid().ToString(),
                    StoreId = storeCustomer.StoreId, // Ăn theo store thực tế của khách hàng
                    CustomerId = storeCustomer.CustomerId,
                    Name = data.Name,
                    Species = data.Species,
                    Color = data.Color,
                    DateOfBirth = data.DateOfBirth
                };
            }).ToList();

            context.Pets.AddRange(pets);
            await context.SaveChangesAsync();
        }

        // 6. Seed Appointments
        private static async Task SeedAppointmentsAsync(AppDbContext context)
        {
            if (context.Appointments.Any()) return;

            var allPets = context.Pets
                .Where(p => p.StoreId != DefaultStoreId)
                .ToList();

            if (!allPets.Any()) return;

            var random = new Random();
            var appointments = new List<Appointment>();

            for (int i = 1; i <= 10; i++)
            {
                var selectedPet = allPets[random.Next(allPets.Count)];

                appointments.Add(new Appointment
                {
                    Id = Guid.NewGuid().ToString(),
                    PetId = selectedPet.Id,
                    CustomerId = selectedPet.CustomerId,
                    StoreId = selectedPet.StoreId, // Đồng bộ đúng chi nhánh mà Pet đăng ký
                    AppointmentDate = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7).AddDays(i)),
                    StartTime = new TimeOnly(8 + (i % 8), 0),
                    EndTime = new TimeOnly(9 + (i % 8), 0),
                    AppointmentNote = $"Lịch hẹn kiểm tra sức khỏe định kỳ lần thứ {i}",
                    Status = (AppointmentStatus)(random.Next(0, 3)),
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    UpdatedAt = DateTime.UtcNow.AddHours(7)
                });
            }

            context.Appointments.AddRange(appointments);
            await context.SaveChangesAsync();
        }

        // 7. Seed Medical Records
        private static async Task SeedMedicalRecordsAsync(AppDbContext context)
        {
            if (context.MedicalRecords.Any()) return;

            // Lấy danh sách các cuộc hẹn đã hoàn thành (Completed) để lập hồ sơ y tế
            // Nếu không có cuộc hẹn nào Completed, lấy đại vài cái để có dữ liệu test
            var completedAppointments = context.Appointments
                .Where(a => a.Status == AppointmentStatus.Completed)
                .ToList();

            if (!completedAppointments.Any())
            {
                completedAppointments = context.Appointments.Take(5).ToList();
            }

            var medicalRecords = new List<MedicalRecord>();

            // Danh sách chẩn đoán và điều trị mẫu để random cho phong phú
            var diagnoses = new[] { "Viêm da dị ứng", "Rối loạn tiêu hóa nhẹ", "Kiểm tra sức khỏe tổng quát", "Tiêm phòng dại định kỳ", "Viêm tai ngoài" };
            var treatments = new[] { "Bôi thuốc mỡ và tắm lá", "Uống men vi sinh", "Không cần điều trị, theo dõi thêm", "Tiêm vaccine Biofel/Rabisin", "Vệ sinh tai bằng dung dịch chuyên dụng" };
            var prescriptions = new[] { "Aderma 20mg", "Enterogermina 5ml", "Vitamin tổng hợp", "Kháng sinh liều nhẹ", "Thuốc nhỏ tai Dexoryl" };

            var random = new Random();

            foreach (var appt in completedAppointments)
            {
                medicalRecords.Add(new MedicalRecord
                {
                    Id = Guid.NewGuid().ToString(),
                    AppointmentId = appt.Id,
                    PetId = appt.PetId, // Lấy luôn PetId từ cuộc hẹn đó
                    Diagnosis = diagnoses[random.Next(diagnoses.Length)],
                    Treatment = treatments[random.Next(treatments.Length)],
                    Prescription = prescriptions[random.Next(prescriptions.Length)],
                    MedicalRecordNote = "Thú cưng hợp tác tốt trong quá trình thăm khám.",
                    CreatedAt = DateTime.UtcNow.AddHours(7)
                });
            }

            context.MedicalRecords.AddRange(medicalRecords);
            await context.SaveChangesAsync();
        }

        // 8. Seed Items (Dịch vụ & Sản phẩm)
        private static async Task SeedItemsAsync(AppDbContext context)
        {
            if (context.Items.Any()) return;

            var basicPlanId = "55555555-5555-5555-5555-555555555555";
            var proPlanId = "66666666-6666-6666-6666-666666666666";
            var businessPlanId = "77777777-7777-7777-7777-777777777777";

            // LẤY DANH SÁCH các store thực tế
            var realStores = context.Stores.Where(s => s.Id != DefaultStoreId).ToList();
            if (!realStores.Any()) return;

            var random = new Random();

            // Định nghĩa các gói Plan (giữ nguyên StoreId = DefaultStoreId hệ thống)
            var items = new List<Item>
            {
                new Item { Id = basicPlanId, StoreId = DefaultStoreId, Name = "Gói Cơ Bản", Price = 500000, Type = ItemType.Plan, DurationInDays = 30 },
                new Item { Id = proPlanId, StoreId = DefaultStoreId, Name = "Gói Chuyên Nghiệp", Price = 2500000, Type = ItemType.Plan, DurationInDays = 180 },
                new Item { Id = businessPlanId, StoreId = DefaultStoreId, Name = "Gói Doanh Nghiệp", Price = 4500000, Type = ItemType.Plan, DurationInDays = 365 }
            };

            // Hàm phụ trợ để bốc ngẫu nhiên 1 store Id thực tế
            string GetRandomStoreId() => realStores[random.Next(realStores.Count)].Id;

            // Thêm các Dịch vụ (Bốc ngẫu nhiên StoreId)
            items.Add(new Item { Id = Guid.NewGuid().ToString(), StoreId = GetRandomStoreId(), Name = "Khám tổng quát", Price = 150000, Type = ItemType.Service });
            items.Add(new Item { Id = Guid.NewGuid().ToString(), StoreId = GetRandomStoreId(), Name = "Tiêm phòng dại (Rabies)", Price = 120000, Type = ItemType.Service });
            items.Add(new Item { Id = Guid.NewGuid().ToString(), StoreId = GetRandomStoreId(), Name = "Combo Tắm & Cắt tỉa lông", Price = 350000, Type = ItemType.Service });
            items.Add(new Item { Id = Guid.NewGuid().ToString(), StoreId = GetRandomStoreId(), Name = "Lưu chuồng (Hotel) - 1 ngày", Price = 200000, Type = ItemType.Service });
            items.Add(new Item { Id = Guid.NewGuid().ToString(), StoreId = GetRandomStoreId(), Name = "Tẩy giun sán", Price = 80000, Type = ItemType.Service });

            // Thêm các Sản phẩm (Bốc ngẫu nhiên StoreId)
            items.Add(new Item { Id = Guid.NewGuid().ToString(), StoreId = GetRandomStoreId(), Name = "Thức ăn hạt Royal Canin 1kg", Price = 250000, Type = ItemType.Product });
            items.Add(new Item { Id = Guid.NewGuid().ToString(), StoreId = GetRandomStoreId(), Name = "Pate cho mèo Whiskas", Price = 15000, Type = ItemType.Product });
            items.Add(new Item { Id = Guid.NewGuid().ToString(), StoreId = GetRandomStoreId(), Name = "Sữa tắm khử mùi cho chó", Price = 180000, Type = ItemType.Product });
            items.Add(new Item { Id = Guid.NewGuid().ToString(), StoreId = GetRandomStoreId(), Name = "Cát vệ sinh đậu nành 6L", Price = 135000, Type = ItemType.Product });
            items.Add(new Item { Id = Guid.NewGuid().ToString(), StoreId = GetRandomStoreId(), Name = "Đồ chơi xương gặm cao su", Price = 45000, Type = ItemType.Product });

            context.Items.AddRange(items);
            await context.SaveChangesAsync();
        }

        // 9. Seed Invoices & InvoiceDetails
        private static async Task SeedInvoicesAsync(AppDbContext context)
        {
            if (context.Invoices.Any()) return;

            var appointments = context.Appointments
                .Where(a => a.Status == AppointmentStatus.Completed && a.StoreId != DefaultStoreId)
                .ToList();

            // Lấy toàn bộ hàng hóa dịch vụ
            var allItems = context.Items.ToList();

            if (!appointments.Any() || !allItems.Any()) return;

            var random = new Random();
            var invoices = new List<Invoice>();

            foreach (var appt in appointments)
            {
                var invoiceId = Guid.NewGuid().ToString();
                var invoiceDetails = new List<InvoiceDetail>();
                decimal total = 0;

                // LỌC CHUẨN: Chỉ lấy những món hàng thuộc CHÍNH STORE ĐÓ hoặc thuộc DEFAULT (Gói Plan hệ thống nếu có mua kèm)
                var storeSpecificItems = allItems
                    .Where(i => i.StoreId == appt.StoreId || i.StoreId == DefaultStoreId)
                    .ToList();

                if (!storeSpecificItems.Any()) continue;

                int numberOfItems = random.Next(1, 4);
                for (int i = 0; i < numberOfItems; i++)
                {
                    var selectedItem = storeSpecificItems[random.Next(storeSpecificItems.Count)];
                    var quantity = random.Next(1, 3);
                    var subtotal = selectedItem.Price * quantity;

                    invoiceDetails.Add(new InvoiceDetail
                    {
                        Id = Guid.NewGuid().ToString(),
                        InvoiceId = invoiceId,
                        ItemId = selectedItem.Id,
                        ItemName = selectedItem.Name,
                        Price = selectedItem.Price,
                        Quantity = quantity,
                        Subtotal = subtotal
                    });

                    total += subtotal;
                }

                invoices.Add(new Invoice
                {
                    Id = invoiceId,
                    StoreId = appt.StoreId, // Đồng bộ chuẩn Store chi nhánh
                    AppointmentId = appt.Id,
                    PetId = appt.PetId,
                    CustomerId = appt.CustomerId,
                    TotalAmount = total,
                    Status = InvoiceStatus.Paid,
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    Details = invoiceDetails
                });
            }

            context.Invoices.AddRange(invoices);
            await context.SaveChangesAsync();
        }

        // 10. Seed Store Package Payments
        private static async Task SeedStorePackagePaymentsAsync(AppDbContext context)
        {
            if (context.StorePackagePayments.Any()) return;

            // Tìm các tài khoản Manager (dựa trên username hoặc logic role của cậu)
            // Ở đây tớ lấy các User có UserName chứa chữ "manager" cho nhanh và chính xác với dữ liệu đã seed
            var managers = context.Users
                .Where(u => u.UserName.Contains("manager"))
                .ToList();

            if (!managers.Any()) return;

            var random = new Random();
            var payments = new List<StorePackagePayment>();

            // Thay vì dùng list cứng, lấy các Item có Type là Plan ra
            var packages = context.Items.Where(i => i.Type == ItemType.Plan).ToList();

            if (!packages.Any()) return;

            foreach (var manager in managers)
            {
                var selectedPackage = packages[random.Next(packages.Count)];

                payments.Add(new StorePackagePayment
                {
                    Id = Guid.NewGuid().ToString(),
                    ManagerId = manager.Id,
                    PackageType = selectedPackage.Name,
                    Price = (double)selectedPackage.Price,
                    DurationInDays = selectedPackage.DurationInDays ?? 30,
                    Status = PaymentStatus.Completed, // Mặc định là đã thanh toán cho đẹp
                    PaymentMethod = "vnpay",
                    TransactionNo = "VNP" + random.Next(100000, 999999).ToString(),
                    PaidAt = DateTime.UtcNow.AddHours(-random.Next(1, 100)),
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    UpdatedAt = DateTime.UtcNow.AddHours(7)
                });
            }

            context.StorePackagePayments.AddRange(payments);
            await context.SaveChangesAsync();
        }

        // 11. Seed Appointment Reminders
        private static async Task SeedAppointmentRemindersAsync(AppDbContext context)
        {
            if (context.AppointmentReminders.Any()) return;

            // Lấy các lịch hẹn đang ở trạng thái Confirmed (Chưa diễn ra) để đặt nhắc hẹn
            var futureAppointments = context.Appointments
                .Where(a => a.Status == AppointmentStatus.Confirmed)
                .ToList();

            if (!futureAppointments.Any()) return;

            var reminders = new List<AppointmentReminder>();
            var random = new Random();

            foreach (var appt in futureAppointments)
            {
                // Giả sử ngày hẹn là AppointmentDate, chúng ta nhắc trước đó 1 ngày hoặc vài tiếng
                var appointmentDateTime = DateTime.SpecifyKind(appt.AppointmentDate.ToDateTime(appt.StartTime), DateTimeKind.Utc);

                reminders.Add(new AppointmentReminder
                {
                    Id = Guid.NewGuid().ToString(),
                    AppointmentId = appt.Id,
                    // Thời gian nhắc là 2 tiếng trước khi cuộc hẹn bắt đầu
                    ReminderTime = appointmentDateTime.AddHours(-2),
                    Status = ReminderStatus.Pending, // Mặc định là đang chờ gửi
                    CreatedAt = DateTime.UtcNow.AddHours(7)
                });
            }

            context.AppointmentReminders.AddRange(reminders);
            await context.SaveChangesAsync();
        }
    }
}
