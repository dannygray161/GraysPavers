var dataTable;

$(document).ready(function() {

    loadDataTable("GetInquiryList")
});

function loadDataTable(url) {
    dataTable = $("#tblData").DataTable({
        "ajax": {
            "url": "/inquiry/" + url
        },
        "columns": [
            {
                "data": "inquiryId",
                "width": "10%"
            },
            {
                "data": "fullName",
                "width": "15%"
            },
            {
                "data": "phoneNumber",
                "width": "15%"
            },
            {
                "data": "email",
                "width": "15%"
            },
            {
                "data": "id",
                "render": function(data) {
                    return `<div class="text-center">
                            <a href="/Inquiry/Details/${data
                        }" class="btn btn-success text-white" style="cursor:pointer">
                                    <i class="fa fa-edit"></i>
                                </a>
                            </div>
                    `;
                },
                "width": "5%"
            }
        ]
    });
};



//if you get an error, make sure you open dev tools, and nav to console then network
//find your call to the api and look at the return to check if the case matches
//what is in your js file.