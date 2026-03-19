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

function previewImage(event) {
    const file = event.target.files[0];
    if (!file) return;

    const reader = new FileReader();
    reader.onload = function (e) {
        const preview = document.getElementById("preview");
        if (preview) {
            preview.src = e.target.result;

            // aseguramos tamaño máximo
            preview.style.maxWidth = "150px";
            preview.style.maxHeight = "150px";
        }
    };
    reader.readAsDataURL(file);
}

function selectProduct(id, name, imageUrl) {

    document.getElementById("ProductId").value = id;

    document.getElementById("selectedProduct").innerHTML = `
                <div class="card shadow-sm p-2" style="max-width:200px;">
                    <img src="${imageUrl}" class="img-fluid" style="height:120px; object-fit:contain;">
                    <div class="text-center mt-2">
                        <strong>${name}</strong>
                    </div>
                </div>
            `;

    var modal = bootstrap.Modal.getInstance(document.getElementById('productModal'));
    modal.hide();
}