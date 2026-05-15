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
        public async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<AppDbContext>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            try { 
                //for identity
                await SeedRolesAsync(roleManager);
                await SeedUsersAsync(userManager);
                await SeedItemsAsync(context);
                Console.WriteLine("Done Identity & Items");

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

            await CreateUserAsync(userManager, "hoang_manager", "hoang.manager@example.com", "Manager@123", "Hoàng", "Nguyễn", "manager");
            await CreateUserAsync(userManager, "lan_anh", "lananh@gmail.com", "Customer@123", "Lan", "Anh", "customer");
            await CreateUserAsync(userManager, "minh_quan", "minhquan@gmail.com", "Customer@123", "Minh", "Quân", "customer");
            await CreateUserAsync(userManager, "thu_thao", "thuthao@gmail.com", "Customer@123", "Thu", "Thảo", "customer");
            await CreateUserAsync(userManager, "quoc_bao", "quocbao@gmail.com", "Customer@123", "Quốc", "Bảo", "customer");
            await CreateUserAsync(userManager, "thanh_truc", "thanhtruc@gmail.com", "Customer@123", "Thanh", "Trúc", "customer");
            await CreateUserAsync(userManager, "gia_huy", "giahuy_test@gmail.com", "Customer@123", "Gia", "Huy", "customer");
            await CreateUserAsync(userManager, "hong_ngoc", "hongngoc@gmail.com", "Customer@123", "Hồng", "Ngọc", "customer");
            await CreateUserAsync(userManager, "tuan_anh", "tuananh_pet@gmail.com", "Customer@123", "Tuấn", "Anh", "customer");
            await CreateUserAsync(userManager, "bich_phuong", "bichphuong@gmail.com", "Customer@123", "Bích", "Phương", "customer");
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

        // 3. Seed Pets
        private static async Task SeedPetsAsync(AppDbContext context)
        {
            if (context.Pets.Any()) return;

            // Lấy danh sách các UserId (Guid) của những người có Role là 'customer'
            // Để đảm bảo Pet được gán cho đúng đối tượng khách hàng
            var customerIds = context.Users.Select(u => u.Id).ToList();

            if (!customerIds.Any()) return;

            var random = new Random();
            var pets = new List<Pet>
            {
                new Pet { Id = Guid.NewGuid().ToString(), Name = "LuLu", Species = "Chó Poodle", Color = "Nâu", DateOfBirth = new DateOnly(2022, 5, 10), CustomerId = customerIds[random.Next(customerIds.Count)] },
                new Pet { Id = Guid.NewGuid().ToString(), Name = "Mimi", Species = "Mèo Anh lông ngắn", Color = "Xám xanh", DateOfBirth = new DateOnly(2023, 1, 15), CustomerId = customerIds[random.Next(customerIds.Count)] },
                new Pet { Id = Guid.NewGuid().ToString(), Name = "Ngáo", Species = "Chó Husky", Color = "Đen trắng", DateOfBirth = new DateOnly(2021, 11, 20), CustomerId = customerIds[random.Next(customerIds.Count)] },
                new Pet { Id = Guid.NewGuid().ToString(), Name = "Bánh Bao", Species = "Mèo Ba Tư", Color = "Trắng", DateOfBirth = new DateOnly(2023, 3, 5), CustomerId = customerIds[random.Next(customerIds.Count)] },
                new Pet { Id = Guid.NewGuid().ToString(), Name = "Xúc Xích", Species = "Chó Dachshund", Color = "Đen vàng", DateOfBirth = new DateOnly(2022, 8, 12), CustomerId = customerIds[random.Next(customerIds.Count)] },
                new Pet { Id = Guid.NewGuid().ToString(), Name = "Kem", Species = "Chó Samoyed", Color = "Trắng tuyết", DateOfBirth = new DateOnly(2022, 12, 25), CustomerId = customerIds[random.Next(customerIds.Count)] },
                new Pet { Id = Guid.NewGuid().ToString(), Name = "Mướp", Species = "Mèo Ta", Color = "Vằn", DateOfBirth = new DateOnly(2020, 6, 30), CustomerId = customerIds[random.Next(customerIds.Count)] },
                new Pet { Id = Guid.NewGuid().ToString(), Name = "Bơ", Species = "Chó Golden Retriever", Color = "Vàng kim", DateOfBirth = new DateOnly(2021, 4, 18), CustomerId = customerIds[random.Next(customerIds.Count)] },
                new Pet { Id = Guid.NewGuid().ToString(), Name = "Đậu Đậu", Species = "Chó Corgi", Color = "Cam trắng", DateOfBirth = new DateOnly(2023, 2, 14), CustomerId = customerIds[random.Next(customerIds.Count)] },
                new Pet { Id = Guid.NewGuid().ToString(), Name = "Mun", Species = "Mèo Munchkin", Color = "Tam thể", DateOfBirth = new DateOnly(2023, 5, 20), CustomerId = customerIds[random.Next(customerIds.Count)] }
            };

            context.Pets.AddRange(pets);
            await context.SaveChangesAsync();
        }

        // 4. Seed Appointments
        private static async Task SeedAppointmentsAsync(AppDbContext context)
        {
            if (context.Appointments.Any()) return;

            // Lấy toàn bộ danh sách Pet đang có (bao gồm cả thông tin CustomerId của nó)
            var allPets = context.Pets.ToList();
            if (!allPets.Any()) return;

            var random = new Random();
            var appointments = new List<Appointment>();

            // Tạo 10 lịch hẹn mẫu
            for (int i = 1; i <= 10; i++)
            {
                // Chọn ngẫu nhiên 1 con pet từ danh sách
                var selectedPet = allPets[random.Next(allPets.Count)];

                appointments.Add(new Appointment
                {
                    Id = Guid.NewGuid().ToString(),
                    PetId = selectedPet.Id,
                    // Quan trọng: CustomerId phải khớp với chủ của con Pet đó
                    CustomerId = selectedPet.CustomerId,
                    AppointmentDate = DateOnly.FromDateTime(DateTime.Now.AddDays(i)), // Lịch hẹn từ mai trở đi
                    StartTime = new TimeOnly(8 + (i % 8), 0), // Giờ bắt đầu từ 8h sáng rải rác ra
                    EndTime = new TimeOnly(9 + (i % 8), 0),
                    AppointmentNote = $"Lịch hẹn kiểm tra sức khỏe định kỳ lần thứ {i}",
                    Status = (AppointmentStatus)(random.Next(0, 3)), // Random status: Confirmed, Completed, Cancelled
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            context.Appointments.AddRange(appointments);
            await context.SaveChangesAsync();
        }

        // 5. Seed Medical Records
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
                    CreatedAt = DateTime.UtcNow
                });
            }

            context.MedicalRecords.AddRange(medicalRecords);
            await context.SaveChangesAsync();
        }

        // 6. Seed Items (Dịch vụ & Sản phẩm)
        private static async Task SeedItemsAsync(AppDbContext context)
        {
            if (context.Items.Any()) return;

            var items = new List<Item>
            {
                // Dịch vụ (Service)
                new Item { Id = Guid.NewGuid().ToString(), Name = "Khám tổng quát", Price = 150000, Type = ItemType.Service },
                new Item { Id = Guid.NewGuid().ToString(), Name = "Tiêm phòng dại (Rabies)", Price = 120000, Type = ItemType.Service },
                new Item { Id = Guid.NewGuid().ToString(), Name = "Combo Tắm & Cắt tỉa lông", Price = 350000, Type = ItemType.Service },
                new Item { Id = Guid.NewGuid().ToString(), Name = "Lưu chuồng (Hotel) - 1 ngày", Price = 200000, Type = ItemType.Service },
                new Item { Id = Guid.NewGuid().ToString(), Name = "Tẩy giun sán", Price = 80000, Type = ItemType.Service },

                // Sản phẩm (Product)
                new Item { Id = Guid.NewGuid().ToString(), Name = "Thức ăn hạt Royal Canin 1kg", Price = 250000, Type = ItemType.Product },
                new Item { Id = Guid.NewGuid().ToString(), Name = "Pate cho mèo Whiskas", Price = 15000, Type = ItemType.Product },
                new Item { Id = Guid.NewGuid().ToString(), Name = "Sữa tắm khử mùi cho chó", Price = 180000, Type = ItemType.Product },
                new Item { Id = Guid.NewGuid().ToString(), Name = "Cát vệ sinh đậu nành 6L", Price = 135000, Type = ItemType.Product },
                new Item { Id = Guid.NewGuid().ToString(), Name = "Đồ chơi xương gặm cao su", Price = 45000, Type = ItemType.Product }
            };

            context.Items.AddRange(items);
            await context.SaveChangesAsync();
        }

        // 7. Seed Invoices & InvoiceDetails
        private static async Task SeedInvoicesAsync(AppDbContext context)
        {
            if (context.Invoices.Any()) return;

            // Lấy danh sách cuộc hẹn và các món hàng (Items) để tính tiền
            var appointments = context.Appointments.Where(a => a.Status == AppointmentStatus.Completed).ToList();
            var items = context.Items.ToList();

            if (!appointments.Any() || !items.Any()) return;

            var random = new Random();
            var invoices = new List<Invoice>();

            foreach (var appt in appointments)
            {
                var invoiceId = Guid.NewGuid().ToString();
                var invoiceDetails = new List<InvoiceDetail>();
                decimal total = 0;

                // Mỗi hóa đơn tớ sẽ cho ngẫu nhiên từ 1 đến 3 món hàng/dịch vụ
                int numberOfItems = random.Next(1, 4);
                for (int i = 0; i < numberOfItems; i++)
                {
                    var selectedItem = items[random.Next(items.Count)];
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
                        Subtotal = subtotal // Chuyển về double cho khớp với property của cậu
                    });

                    total += subtotal;
                }

                invoices.Add(new Invoice
                {
                    Id = invoiceId,
                    AppointmentId = appt.Id,
                    PetId = appt.PetId,
                    CustomerId = appt.CustomerId,
                    TotalAmount = total,
                    CreatedAt = DateTime.UtcNow,
                    Details = invoiceDetails // EF Core sẽ tự động lưu cả Details vào database
                });
            }

            context.Invoices.AddRange(invoices);
            await context.SaveChangesAsync();
        }

        // 8. Seed Store Package Payments
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

            // Các gói dịch vụ mẫu
            var packages = new[]
            {
                new { Name = "Gói Cơ Bản (1 Tháng)", Price = 500000.0 },
                new { Name = "Gói Chuyên Nghiệp (6 Tháng)", Price = 2500000.0 },
                new { Name = "Gói Doanh Nghiệp (1 Năm)", Price = 4500000.0 }
            };

            foreach (var manager in managers)
            {
                var selectedPackage = packages[random.Next(packages.Length)];

                payments.Add(new StorePackagePayment
                {
                    Id = Guid.NewGuid().ToString(),
                    ManagerId = manager.Id,
                    PackageType = selectedPackage.Name,
                    Price = selectedPackage.Price,
                    Status = PaymentStatus.Completed, // Mặc định là đã thanh toán cho đẹp
                    PaymentMethod = "vnpay",
                    TransactionNo = "VNP" + random.Next(100000, 999999).ToString(),
                    PaidAt = DateTime.UtcNow.AddHours(-random.Next(1, 100)),
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    UpdatedAt = DateTime.UtcNow
                });
            }

            context.StorePackagePayments.AddRange(payments);
            await context.SaveChangesAsync();
        }

        // 9. Seed Appointment Reminders
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
                    CreatedAt = DateTime.UtcNow
                });
            }

            context.AppointmentReminders.AddRange(reminders);
            await context.SaveChangesAsync();
        }
    }
}
