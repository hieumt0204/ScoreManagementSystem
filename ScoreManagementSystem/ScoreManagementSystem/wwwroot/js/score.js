//document.addEventListener("DOMContentLoaded", function () {
//    Swal.fire({
//        icon: "error",
//        title: "Oops...",
//        text: "Something went wrong!",
//        footer: '<a href="#">Why do I have this issue?</a>'
//    });
//});

$(document).ready(function () {
    
    $('#subjectSelect').change(function () {
        var selectedSubjectId = $(this).val();

        $.ajax({
            url: '/Scores/Index?handler=LoadClassBySubject',
            type: 'GET',
            data: { subjectId: selectedSubjectId },
            success: function (data) {

                $('#classSelect').empty();
                $('#classSelect2').empty();

                $.each(data.classes, function (index, item) {
                    $('#classSelect').append($('<option>', {
                        value: item.id,
                        text: item.name
                    }));

                    $('#classSelect2').append($('<option>', {
                        value: item.id,
                        text: item.name
                    }));
                });

                $('#componentScoreSelect').empty();
                $('#componentScoreSelect').append($('<option>', {
                    value: -1,
                    text: 'All'
                }));
                $.each(data.componentScores, function (index, item) {
                    $('#componentScoreSelect').append($('<option>', {
                        value: index+1,
                        text: item.name
                    }));
                });

            },
            error: function (error) {
                console.log(error);
            }
        });
    });

    $('#searchText').on('input', function () {
        var searchText = $(this).val().trim().toLowerCase();

        $('#exampleTable tbody tr').each(function () {
            var idText = $(this).find('td:first').text().toLowerCase();
            var nameText = $(this).find('td:eq(1)').text().toLowerCase();

            if (idText.includes(searchText) || nameText.includes(searchText)) {
                $(this).show();
            } else {
                $(this).hide();
            }
        });
    });

    $('#componentScoreSelect').change(function () {
        var selectedComponent = $(this).val();

        if (selectedComponent != -1) {
            selectedComponent = parseInt(selectedComponent, 10);
            selectedComponent += 2;
            var totalColumns = $('#exampleTable thead tr th').length;

            for (var i = 1; i < totalColumns; i++) {
                if (i !== 1 && i !== 2 && i !== selectedComponent) {
                    $('#exampleTable').find("tbody tr, thead tr")
                        .children(":nth-child(" + i + ")")
                        .hide();
                } else {
                    $('#exampleTable').find("tbody tr, thead tr")
                        .children(":nth-child(" + i + ")")
                        .show();
                }
            }
        } else {
            $('#exampleTable').find("tbody tr, thead tr")
                .children()
                .show();
        }

        console.log(selectedComponent);
    });

    $('#saveButton').click(function () {
        var data = [];
        var isValid = true;

        $('#exampleTable tbody tr').each(function () {

            var studentName = $(this).find('.student-name').text();

            $(this).find('.component-score').each(function () {
                var score = {};
                var point = $(this).find('[name="score"]').val();
                var componentScore = $(this).find('[name="componentScore"]').val();
                var classStudent = $(this).find('[name="classStudent"]').val();

                if (point != "" && (point < 0 || point > 10)) {
                    alert("Score of " + studentName + " is must be in range 0 to 10!");
                    isValid = false;
                    return false;
                }
                point = point === "" ? -1 : point;

                score['Score1'] = parseFloat(point);
                score['ComponentScoreId'] = parseInt(componentScore, 10);
                score['StudentId'] = parseInt(classStudent, 10);

                data.push(score);
            });
            if (!isValid)
                return false;
        });
        if (!isValid)
            return;
        console.log(data);
        $.ajax({
            url: '/Scores/Index?handler=SaveScore',
            headers:
            {
                "RequestVerificationToken": $('input:hidden[name="__RequestVerificationToken"]').val()
            },
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            success: function (response) {
                alert(response.message);
                console.log(response);
            },
            error: function (error) {
                console.log(error);
            }
        });
    });

    $('#getTotalButton').click(function () {
        $('#exampleTable tbody tr').each(function () {
            var row = $(this);
            var scoreInputs = row.find('input[name="score"]');
            var totalScore = 0;

            scoreInputs.each(function () {
                var scoreValue = $(this).val();
                var percentInput = $(this).closest('td').find('input[name="percent"]');
                var percentValue = percentInput.val() / 100;
                console.log('a', percentValue);
                if (scoreValue !== "") {
                    totalScore += parseFloat(scoreValue) * parseFloat(percentValue);
                    
                }
            });

            row.find('td:last-child').text(totalScore.toFixed(2));
        });
    });

});