var Sitemap = {
    _interval: null,

    init: function () {
        /** Initializes the object. */

        var self = Sitemap;

        $('.form button').click(function () {
            self.beginParseSitemap($('.form input').val());
        });

        $('.form input').keypress(function (e) {
            if (e.keyCode == 13) { /* Enter */
                e.preventDefault();
                self.beginParseSitemap($(e.target).val());
            }
        });
    },

    beginParseSitemap: function (url) {
        /** 
         * Begins the process of sitemap parsing.
         * @param {string} url Website URL.
         */

        var self = this;

        if (url && url.length) {
            /* Disabling form controls */
            this._editable(false);

            /* Dims current results indicating that they might be outdated */
            $('#sitemap').addClass('dimmed');

            $.get('/api/Sitemap/BeginParseSitemap?url=' + encodeURIComponent(url), function () {
                self._interval = setInterval(function () {
                    self.getProgress(url);
                }, 1000);
            });
        }
    },

    getProgress: function (url) {
        /** 
         * Updates sitemap parsing progress.
         * @param {string} url Website URL.
         */

        var self = this;

        $.get('/api/Sitemap/GetProgress?url=' + encodeURIComponent(url), function (percentage) {
            $('#progress').attr('value', percentage);

            if (percentage == 100) {
                clearInterval(Sitemap._interval);
                self.getResult(url);
            }
        });
    },

    getResult: function (url) {
        /** 
         * Queries and displays sitemap.
         * @param {string} url Website URL.
         */

        var self = this;

        $.get('/api/Sitemap/GetResult?url=' + encodeURIComponent(url), function (result) {
            $('#progress').attr('value', 0);
            self._editable(true);

            self.renderSitemap(result);
        });
    },

    renderSitemap: function (sitemap) {
        /** 
         * Renders sitemap.
         * @param {object} sitemap Sitemap object.
         */

        var c = $('#sitemap').removeClass('dimmed').addClass('active').empty();

        if (sitemap.Children.length) {
            c.append(this._renderLevel(sitemap, $('<ul />')));
        } else {
            c.html('<i>Sitemap is empty.</i>');
        }
    },

    _renderLevel: function (node, container) {
        /** 
         * Renders sitemap level.
         * @param {object} node Sitemap node.
         * @param {object} container DOM node representing container for the given node.
         */

        var item = function (n) {
            return $('<li />').append($('<a />').attr('href', n.Url).text(n.Title || n.Url));
        }, parent = null, inner = null;

        container.append(parent = item(node));

        if (node.Children) {
            parent.append(inner = $('<ul />'));

            for (var i = 0; i < node.Children.length; i++) {
                this._renderLevel(node.Children[i], inner);
            }
        }

        return container;
    },

    _editable: function (val) {
        /** 
         * Changes the "editable" state of the form.
         * @param {boolean} val Value indicating whether form is enabled.
         */

        $('.form button, .form input').prop('disabled', !val);
    }
};

/* Initializing the object once DOM is ready */
$(Sitemap.init);
