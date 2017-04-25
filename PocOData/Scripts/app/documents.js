var feedUrl = "odata/Documents";

window.viewModel = (function () {
    var self = this;

    self.documents = ko.observableArray();
    self.title = ko.observable();
    self.errorMessage = ko.observable();
    
    self.createDocument = function () {
        ajaxRequest("post", "odata/CreateDocument", { Title: self.title() })
            .done(function (result) {
                self.documents.push(new Document(result));
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                self.errorMessage(errorThrown);
            });
    }
    
    self.checkoutMany = function () {
        var keys = {};
        var data = {
            DocumentIds: $.map(self.documents(), function (document) {
                if (document.selected()) {
                    keys[document.id] = document;
                    return document.id;
                }
            })
        };
        ajaxRequest("post", feedUrl + "/PocOData.Data.Models.CheckOutMany", data)
            .done(function (result) {
                $.each(result.value, function (index, m) {
                    keys[m.ID].update(m);
                });
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                self.errorMessage(errorThrown);
            });
    }
    
    function Document(data, parent) {
        var self = this;

        self.id = data.ID;
        self.title = data.Title;
        self.isCheckedOut = ko.observable();
        self.checkoutUrl = ko.observable();
        self.returnDocumentUrl = ko.observable();
        self.selected = ko.observable();

        self.update = updateDocument;

        self.checkout = function () {
            invokeAction(self.checkoutUrl());
        };

        self.returnDocument = function () {
            invokeAction(self.returnDocumentUrl());
        };
        
        function invokeAction(url) {
            ajaxRequest("post", url)
                .done(function (updated) {
                    updateDocument(updated);
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    parent.errorMessage(errorThrown);
                });
        }

        self.update(data);
        
        function updateDocument(data) {
            self.isCheckedOut(data.isCheckedOut);

            if (data["#ODataActionsSample.Models.CheckOut"]) {
                self.checkoutUrl(data["#PocOData.Data.Models.CheckOut"].target);
            }
            else {
                self.checkoutUrl(null);
            }
            if (data["#PocOData.Data.Models.Return"]) {
                self.returnDocumentUrl(data["#PocOData.Data.Models.Return"].target);
            }
            else {
                self.returnDocumentUrl(null);
            }
        }
    }
    
    $.ajaxSetup({
        accepts: { "json": "application/json;odata.metadata=full" }
    });
    
    ajaxRequest("get", feedUrl)
        .done(function (data) {
            var mapped = $.map(data.value, function (element) { return new Document(element, self); });
            self.documents(mapped);
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            self.errorMessage(errorThrown);
        });
    
    function ajaxRequest(type, url, data) {
        var options = {
            dataType: "json",
            contentType: "application/json",
            type: type,
            data: data ? ko.toJSON(data) : null
        };
        self.errorMessage(null);
        return $.ajax(url, options);
    }
})();

$(document).ready(function () {
    ko.applyBindings(viewModel);
});