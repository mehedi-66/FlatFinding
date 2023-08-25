using FlatFinding.Models;
using Microsoft.Extensions.Primitives;
using System.Text;

namespace FlatFinding.ReportTemplate
{
    public static class InvoiceHtmlTemplate
    {
        public static string GetHtml(JoinedFlatBookingData Data, string Header)
        {
           
            var sb = new StringBuilder();
            sb.AppendFormat(@"
                        <html>
                            <head>
                            </head>
                            <body>
                                <div class='header'><h1>Booking Invoice</h1></div>
                                 <h3 align='right'>Date: <span id='datetime'></span> </h3>");
            
            
                                  
           
                sb.AppendFormat(@" <hr/>
                                   <p><b>Payment:</b> Paid</p>
                                   <p><b>Flat Name: </b> {0}</p>
                                   <p><b>Flat Address: </b> {1}</p>
                                   <p><b>Flat Type: </b>{2}</p>
                                   <p><b>Flat Cost: </b>{3} per/Month</p>
                                   <p><b>Flat Owner Name: </b>{4}</p>
                                   <p><b>Flat Owner Phone: </b>{5}</p>
                                    <hr/>
                                   <p><b>Booking Date: </b>{6}</p>
                                   <p><b>Booked By: </b>{7}</p>
                                   <p><b>Phone Number: </b>{8}</p>
                                   
                                   
                                    
                                  ", 
                                  Data.FlatName, 
                                  Data.Address,
                                  Data.Type, 
                                  Data.FlatCost,
                                  Data.OwnerName, 
                                  Data.OwnerPhone, 
                                  Data.BookingDate?.ToString("dd-MM-yyyy"),
                                  Data.BuyerName,
                                  Data.BuyerPhone
                                  );
            
            sb.Append(@"</body> <script>
        const datetimeElement = document.getElementById('datetime');
        const currentDatetime = new Date().toLocaleString();
        datetimeElement.textContent = currentDatetime;
    </script>


      </html>");
            return sb.ToString();
        }
    }
}
