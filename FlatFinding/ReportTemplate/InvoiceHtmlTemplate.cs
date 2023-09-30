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
                                    <hr/>
                                    <p>Police Verification Information </p>
                                     <p><b>Who Live Here His Name: </b>{9}</p>
                                     <p><b>Father Name: </b>{10}</p>
                                     <p><b>Data of Birth: </b>{11}</p>
                                     <p><b>Parmanent Address: </b>{12}</p>
                                     <p><b>Marital Status: </b>{13}</p>
                                     <p><b>NID: </b>{14}</p>
                                     <p><b>Religion: </b>{15}</p>
                                     <p><b>Person 1 Name: </b>{16}</p>
                                     <p><b>Person 1 Phone: </b>{17}</p>
                                     <p><b>Person 2 Name: </b>{18}</p>
                                     <p><b>Person 2 Phone: </b>{19}</p>
                                     <p><b>What is the Reason Previous House Leave: </b>{20}</p>
                                     <p><b>How Many Month Do You want to live Here: </b>{21}</p>
                                   
                                   
                                    
                                  ", 
                                  Data.FlatName, 
                                  Data.Address,
                                  Data.Type, 
                                  Data.FlatCost,
                                  Data.OwnerName, 
                                  Data.OwnerPhone, 
                                  Data.BookingDate?.ToString("dd-MM-yyyy"),
                                  Data.BuyerName,
                                  Data.BuyerPhone,
                                  Data.NameOfWhoLive,
                                  Data.FatherName,
                                  Data.DateOfBirth,
                                  Data.ParmanetAddress,
                                  Data.MaritalStatus,
                                  Data.NID,
                                  Data.Religion,
                                  Data.Person1Name,
                                  Data.Person1Phone,
                                  Data.Person2Name,
                                  Data.Person2Phone,
                                  Data.ResoneOfLeveingHouse,
                                  Data.Condition

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
