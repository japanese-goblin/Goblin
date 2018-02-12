$(".send_message").click(function () {
    var message = $(".message").val().replace(/\n/g, "<br>");
    $.ajax({
        type: "GET",
        url: "/Api/SendMessage",
        async: true,
        data: "msg=" + message,
        success: function () {
            $(".send_message").removeClass("btn-info").addClass("btn-success");
        },
        error: function () {
            $(".send_message").removeClass("btn-info").addClass("btn-danger");
        }
    });
});

$(".get_messages").click(function () {
    $.ajax({
        type: "GET",
        url: "/api/api.php",
        async: true,
        data: "method=showmsgs&count=" + $(".messages_count").val() + "&toTable=1",
        success: function (html) {
            $(".messages").html(html);
        }
    });
});

$(".show_tests").click(function () {
    $(".show_tests").text("Loading...");
    $(".show_tests").prop("disabled", true);
    $.ajax({
        type: "GET",
        url: "../test/show.php",
        async: true,
        success: function (html) {
            $(".alltests").html(html);
            $(".show_tests").prop("disabled", false);
            $(".show_tests").text("Запустить тесты");
        },
        error: function (html) {
            $(".show_tests").addClass("btn-danger");
            $(".show_tests").prop("disabled", false);
            $(".show_tests").text("Запустить тесты");
        }
    });
});
