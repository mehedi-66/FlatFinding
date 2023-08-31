using FlatFinding.Models;
using Microsoft.Extensions.Primitives;
using System.Text;

namespace FlatFinding.ReportTemplate
{
    public static class BookedHtmlTemplate
    {
        public static string GetHtml(List<JoinedFlatBookingData> joinedData, string Header)
        {
            int TotalBooking = 0;
            float TotalSpend = 0;
            float TotalProfit = 0;
            foreach(var temp in joinedData)
            {
                TotalBooking++;
                TotalSpend += temp.FlatCost;
                TotalProfit += temp.FlatProfit;
            }

            var sb = new StringBuilder();
            sb.AppendFormat(@"
                        <html>
                            <head>
                            </head>
                            <body>
                                <div class='header'><h1>{0} Booking List</h1></div>
                                 <h3 align='right'>Date: <span id='datetime'></span> </h3>
                                 <h3 align='left'>Total Booking:", Header);
            
            sb.AppendFormat(@"<span style='margin-right: 20px' id='booking'> {0}</span>
                                    Total Spend: 
                                    <span style='margin-right: 20px' id='spend'> {1}</span>
                                    Total Profit: 
                                    <span style='margin-right: 8px' id='profit'> {2}</span>
                                </h3>
                                <table align='center'>
                                    <tr>
                                        <th>Sl</th>
                                        <th>Flat Name</th>
                                        <th>Flat Address</th>
                                        <th>Owner</th>
                                        <th>Buyer</th>
                                        <th>Flat Cost</th>
                                        <th>Profit</th>
                                        <th>booking Date</th>
                                        <th>booking Cancel</th>
                                    </tr>""", TotalBooking, TotalSpend, TotalProfit);
                                  
            int cnt = 0;
            foreach (var emp in joinedData)
            {
                cnt++;
                sb.AppendFormat(@"<tr>
                                    <td>{0}</td>
                                    <td>{1}</td>
                                    <td>{2}</td>
                                    <td>{3}</td>
                                    <td>{4}</td>
                                    <td>{5}</td>
                                    <td>{6}</td>
                                    <td>{7}</td>
                                    <td>{8}</td>
                                   
                                    
                                  </tr>",cnt, emp.FlatName, emp.Address,
                                           emp.OwnerName, emp.BuyerName, 
                                           emp.FlatCost, emp.FlatProfit, emp.BookingDate?.ToString("dd-MM-yyyy"), emp.BookingCancel?.ToString("dd-MM-yyyy"));
            }
            sb.Append(@"
                                </table>
                            </body> <script>
        const datetimeElement = document.getElementById('datetime');
        const currentDatetime = new Date().toLocaleString();
        datetimeElement.textContent = currentDatetime;
    </script>


      </html>");
            return sb.ToString();
        }
    }
}
