(function () {
    angular.module('RemoteManagement').directive('popup', function () {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                element.bind('mouseenter', function () {
                    var $this = $(this);
                    var $parent = $this.closest('.tab-pane');

                    var off1 = $parent.offset();
                    var w1 = $parent.width();

                    var off2 = $this.offset();
                    var w2 = $this.width();

                    var place = 'left';
                    if (parseInt(off2.left) < parseInt(off1.left) + parseInt(w1 / 2)) place = 'right';

                    $this.find('.popover').removeClass('right left').addClass(place);
                }).on('click', function () {
                    return false;
                });
            }
        };
    });
})();