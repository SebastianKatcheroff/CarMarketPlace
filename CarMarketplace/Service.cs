using CarMarketplace.DAO;
using CarMarketplace.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CarMarketplace
{
    public class Service
    {
        AppDbContext context;

        public Service()
        {
            context = new AppDbContext();
        }

        public void Test()
        {
            var lista = context.Buyer.ToList();
        }

        public async Task GetData()
        {
            var query =
            from q in context.Quote
            where q.IsCurrent
            join c in context.Car on q.CarId equals c.CarId
            join b in context.Buyer on q.BuyerId equals b.BuyerId
            join z in context.ZipCode on q.ZipCodeId equals z.ZipCodeId
            join csm in context.CarSubModel on c.CarSubModelId equals csm.CarSubModelId
            join cm in context.CarModel on csm.CarModelId equals cm.CarModelId
            join cmk in context.CarMake on cm.CarMakeId equals cmk.CarMakeId
            let latestStatus = context.QuoteStatus
                .Where(qs => qs.QuoteId == q.QuoteId)
                .OrderByDescending(qs => qs.StatusDate)
                .FirstOrDefault()
            join l in context.Lookup on latestStatus.StatusId equals l.LookupId
            orderby z.ZipCodeId, c.CarId, q.QuoteId
            select new
            {
                MakeName = cmk.Name,
                ModelName = cm.Name,
                SubModelName = csm.Name,
                Year = c.Year,
                ZipCode = z.Code,
                BuyerName = b.Name,
                CurrentOffer = q.Amount,
                CurrentStatus = l.DisplayName,
                CurrentStatusDate = latestStatus.StatusDate
            };

            var result = query.AsNoTracking().ToList();
        }

        //Original code:
        public void UpdateCustomersBalanceByInvoices(List<Invoice> invoices)
        {
            //foreach (var invoice in invoices)
            //{
            //    var customer = context.Customers.SingleOrDefault(invoice.CustomerId.Value);
            //    customer.Balance -= invoice.Total;
            //    context.SaveChanges();
            //}
        }

        //Changes to make it better.
        public async Task UpdateCustomersBalanceByInvoicesAsync(List<Invoice> invoices)
        {
            //1) Validate invoice list
            if (invoices == null || !invoices.Any())
                return;

            //2) Optional: if we are not sure of the data source
            //Filter invoices with valid customer ID and group by customer
            //If invoice.Total=0 it does not subtract from the balance
            //And Validate new invoice list
            var validInvoices = invoices.Where(i => i.CustomerId > 0 && i.Total > 0).ToList();

            if (!validInvoices.Any())
                return;

            //3) Extract list of IDs from invoices
            var customerIds = validInvoices.Select(i => i.CustomerId).Distinct().ToList();

            //4) Get all clients in a single query
            var customers = await context.Customers
                .Where(c => customerIds.Contains(c.Id))
                .ToListAsync();

            //5) Extract totals per customer
            var customerUpdates = from customer in customers
                                  join total in validInvoices
                                      .GroupBy(i => i.CustomerId)
                                      .Select(g => new { CustomerId = g.Key, Total = g.Sum(i => i.Total) })
                                      on customer.Id equals total.CustomerId
                                  select new { customer, total.Total };

            //Update balances
            foreach (var update in customerUpdates)
            {
                update.customer.Balance -= update.Total;
            }

            //Save all changes in a single transaction and not for each iteration
            await context.SaveChangesAsync();
        }

        public async Task<List<OrderDTO>> GetOrders(
            DateTime? dateFrom,
            DateTime? dateTo,
            List<int> customerIds,
            List<int> statusIds,
            bool? isActive)
        {
            var query = context.Orders.AsQueryable();

            if (dateFrom.HasValue)
                query = query.Where(o => o.OrderDate >= dateFrom);

            if (dateTo.HasValue)
                query = query.Where(o => o.OrderDate <= dateTo);

            if (customerIds?.Any() ?? false)
                query = query.Where(o => customerIds.Contains(o.CustomerId));

            if (statusIds?.Any() ?? false)
                query = query.Where(o => statusIds.Contains(o.StatusId));

            if (isActive.HasValue)
                query = query.Where(o => o.IsActive == isActive);

            return await query
                .Select(o => new OrderDTO
                {
                    Id = o.Id,
                    Date = o.OrderDate,
                    CustomerId = o.CustomerId,
                    StatusId = o.StatusId,
                    IsActive = o.IsActive
                })
                .ToListAsync();
        }


        //public async Task ProcessCsFiles(string rootPath)
        //{
        //    var files = Directory.GetFiles(rootPath, "*.cs", SearchOption.AllDirectories);

        //    var asyncMethodRegex = new Regex(@"async\s+(?:Task|ValueTask)(?:<[^>]+>)?\s+(\w+)\s*\(", RegexOptions.Compiled);
        //    //var suffixRegex = new Regex(@"\b(Vm|Vms|Dto|Dtos)\b", RegexOptions.Compiled);
        //    var suffixRegex = new Regex(@"(Vm|Vms|Dto|Dtos)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        //    //var methodStartRegex = new Regex(@"^\s*(public|private|protected|internal)\s+.*\(", RegexOptions.Compiled);
        //    var methodStartRegex = new Regex(@"^\s*(public|private|protected|internal)\s+.*\(", RegexOptions.Compiled);

        //    foreach (var file in files)
        //    {
        //        var lines = File.ReadAllLines(file).ToList();
        //        var modified = false;

        //        for (int i = 0; i < lines.Count; i++)
        //        {
        //            var line = lines[i];

        //            var match = asyncMethodRegex.Match(line);
        //            if (match.Success)
        //            {
        //                var methodName = match.Groups[1].Value;
        //                if (!methodName.EndsWith("Async"))
        //                {
        //                    lines[i] = line.Replace(methodName, methodName + "Async");
        //                    modified = true;
        //                }
        //            }

        //            var replaced = suffixRegex.Replace(lines[i], m =>
        //            {
        //                return m.Value switch
        //                {
        //                    "Vm" => "VM",
        //                    "Vms" => "VMs",
        //                    "Dto" => "DTO",
        //                    "Dtos" => "DTOs",
        //                    _ => m.Value
        //                };
        //            });

        //            if (replaced != lines[i])
        //            {
        //                lines[i] = replaced;
        //                modified = true;
        //            }
        //        }

        //        for (int i = 0; i < lines.Count; i++)
        //        {
        //            // Detecta si la línea tiene cierre de método pegado a otra firma
        //            var match = Regex.Match(lines[i], @"\}\s*(public|private|protected|internal)");
        //            if (match.Success)
        //            {
        //                // Separar la línea en dos
        //                var line = lines[i];
        //                var splitIndex = line.IndexOf(match.Groups[1].Value);
        //                lines[i] = line.Substring(0, splitIndex).TrimEnd(); // hasta la llave
        //                lines.Insert(i + 1, line.Substring(splitIndex).TrimStart()); // nueva línea con la firma
        //                i++; // saltar la línea insertada
        //                modified = true;
        //            }
        //        }

        //        for (int i = 1; i < lines.Count; i++)
        //        {
        //            if (methodStartRegex.IsMatch(lines[i]))
        //            {
        //                var prevLine = lines[i - 1].Trim();
        //                if (!string.IsNullOrWhiteSpace(prevLine) &&
        //                    !prevLine.StartsWith("[") &&
        //                    !prevLine.EndsWith("{"))
        //                {
        //                    lines.Insert(i, "");
        //                    i++;
        //                    modified = true;
        //                }
        //            }
        //        }


        //        if (modified)
        //        {
        //            File.WriteAllLines(file, lines);
        //        }
        //    }
        //}

        public async Task ProcessCsFiles(string rootPath)
        {
            if (!Directory.Exists(rootPath))
                throw new DirectoryNotFoundException($"Directory not found: {rootPath}");

            var files = Directory.GetFiles(rootPath, "*.cs", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                await ProcessFileAsync(file);
            }
        }

        private async Task ProcessFileAsync(string filePath)
        {
            var lines = (await File.ReadAllLinesAsync(filePath)).ToList();
            var originalContent = string.Join(Environment.NewLine, lines);
            var modified = false;

            modified |= RenameAsyncMethodsWithoutSuffix(lines);
            modified |= RenamePatternsInLines(lines);
            modified |= FixMethodClosingBraces(lines);
            modified |= AddBlankLinesBetweenMethods(lines);

            if (modified)
            {
                var newContent = string.Join(Environment.NewLine, lines);
                if (originalContent != newContent)
                {
                    await File.WriteAllLinesAsync(filePath, lines);
                }
            }
        }

        private bool RenameAsyncMethodsWithoutSuffix(List<string> lines)
        {
            var asyncMethodRegex = new Regex(@"async\s+(?:Task|ValueTask)(?:<[^>]+>)?\s+(\w+)\s*\(", RegexOptions.Compiled);
            var modified = false;

            for (int i = 0; i < lines.Count; i++)
            {
                var match = asyncMethodRegex.Match(lines[i]);
                if (match.Success)
                {
                    var methodName = match.Groups[1].Value;
                    if (!methodName.EndsWith("Async"))
                    {
                        lines[i] = lines[i].Replace(methodName, methodName + "Async");
                        modified = true;
                    }
                }
            }

            return modified;
        }

        private bool RenamePatternsInLines(List<string> lines)
        {
            var suffixRegex = new Regex(@"(Vm|Vms|Dto|Dtos)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var modified = false;

            for (int i = 0; i < lines.Count; i++)
            {
                var replaced = suffixRegex.Replace(lines[i], match =>
                {
                    return match.Value.ToLower() switch
                    {
                        "vm" => "VM",
                        "vms" => "VMs",
                        "dto" => "DTO",
                        "dtos" => "DTOs",
                        _ => match.Value
                    };
                });

                if (replaced != lines[i])
                {
                    lines[i] = replaced;
                    modified = true;
                }
            }

            return modified;
        }

        private bool FixMethodClosingBraces(List<string> lines)
        {
            var closingBraceRegex = new Regex(@"\}\s*(public|private|protected|internal)", RegexOptions.Compiled);
            var modified = false;

            for (int i = 0; i < lines.Count; i++)
            {
                var match = closingBraceRegex.Match(lines[i]);
                if (match.Success)
                {
                    var line = lines[i];
                    var splitIndex = line.IndexOf(match.Groups[1].Value);

                    lines[i] = line[..splitIndex].TrimEnd();
                    lines.Insert(i + 1, line[splitIndex..].TrimStart());

                    i++;
                    modified = true;
                }
            }

            return modified;
        }

        private bool AddBlankLinesBetweenMethods(List<string> lines)
        {
            var methodStartRegex = new Regex(@"^\s*(public|private|protected|internal)\s+.*\(", RegexOptions.Compiled);
            var modified = false;

            for (int i = 1; i < lines.Count; i++)
            {
                if (methodStartRegex.IsMatch(lines[i]))
                {
                    var previousLine = lines[i - 1].Trim();

                    if (!string.IsNullOrWhiteSpace(previousLine) &&
                        !previousLine.StartsWith("[") &&
                        !previousLine.EndsWith("{"))
                    {
                        lines.Insert(i, string.Empty);
                        i++;
                        modified = true;
                    }
                }
            }

            return modified;
        }

    }
}
