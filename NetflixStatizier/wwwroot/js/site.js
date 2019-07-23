﻿$(() => {
    $('[data-toggle="popover"]').popover();
});

$(document).ready(function () {
    const securityToken = $('[name=__RequestVerificationToken]').val();
    $(document).ajaxSend((event, request, opt) => {
        if (opt.hasContent && securityToken) {   // handle all verbs with content
            var tokenParam = "__RequestVerificationToken=" + encodeURIComponent(securityToken);
            opt.data = opt.data ? [opt.data, tokenParam].join("&") : tokenParam;
            // ensure Content-Type header is present!
            if (opt.contentType !== false || event.contentType) {
                request.setRequestHeader("Content-Type", opt.contentType);
            }
        }
    });
});