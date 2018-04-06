$(document).ready(function() {
        //find all images, but not the logo, and add the lightbox
        $('img').not('#logo').each(function(){
            var $img = $(this);
            var filename = $img.attr('src')
            //add cursor
            $img.css('cursor','zoom-in');
            $img.css('cursor','-moz-zoom-in');
            $img.css('cursor','-webkit-zoom-in');

            //add featherlight
            $img.attr('alt', filename);
            $img.featherlight(filename);
    });
});