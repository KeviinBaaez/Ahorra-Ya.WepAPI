// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function confirm(id, name, propertie) {
    Swal.fire({
        title: `Remove ${propertie}`,
        html: `Are you sure to remove the ${propertie} <strong>"${name}</strong>"?`,
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            document.getElementById(`formDelete-${id}`).submit();
        }
    });
}