if (typeof(noext) === 'undefined') {
   noext = {}; 
}

if (typeof jQuery === 'undefined') {
    throw new Error('noext.Menu: jQuery must be present');
}

// Constructor
noext.Menu = function (n, subMenuStyle) {
    this.n = n;
    this.subMenuStyle = (subMenuStyle === 'ext') ? 'opened-ext' : 'opened';
    this.firstLi = this.n.children('li');
    this.subMenu = this.firstLi.find('ul');
    this.subMenuItems = this.subMenu.find('li');
    this.init();
    noext.Menu.menuInstances.push(this);
};
noext.Menu.menuInstances = [];

noext.Menu.prototype = {
    init: function () { // add events to what we need
        var that = this;
        this.subMenuItems.mouseover( this.hover); // do mouseovers for ie6
        this.subMenuItems.mouseout( this.unHover);
        this.n.click( function (event) { that.update(event);}); // update the menu
        
        jQuery(document).click( function (event) { that.closeAllSubMenus(event); }); // close other menus when we click one
    },
    update: function (e) {
            var t = jQuery(e.target);          
            // don't follow link if first item, whose target is '<u>'
            if (t.hasClass('first-link') ) {
                e.preventDefault();
            }

            var sm = this.subMenu;
            // close other menus
            this.closeOtherSubMenus(sm);
       
            var subMenuStyle = this.subMenuStyle;
            
            if ( !sm.hasClass(subMenuStyle)) {
                sm.addClass(subMenuStyle);
            } else {
                sm.removeClass(subMenuStyle);
            }
            e.stopPropagation(); // don't let default event occur (removeMenus from document.click)

            

    },
    hover: function () {
        jQuery(this).addClass('active-item');

    },
    unHover: function () {
        jQuery(this).removeClass('active-item');

    },
    closeOtherSubMenus: function (currentSubMenu) {
        // close all menu's submenus, that are different than currently clicked menu's submenu
        var subMenuStyle = this.subMenuStyle;
        currentSubMenu = this.subMenu;
        var menuInstances = noext.Menu.menuInstances;
        for (var i = 0; i < menuInstances.length; i++) {
            var menuInstance = menuInstances[i];
     
            if (currentSubMenu !== menuInstance.subMenu && menuInstance.subMenu.hasClass(subMenuStyle)) {
                menuInstance.subMenu.removeClass(subMenuStyle);
            } 
        }
    },
    closeAllSubMenus: function (event) {
        // for when user clicks on document
        var menuInstances = noext.Menu.menuInstances;
        for (var i = 0; i < menuInstances.length; i++) {
            var subMenu = this.subMenu;
            subMenu.removeClass(this.subMenuStyle);
        }
    }
};
// end of menu objecta


jQuery(document).ready( function () {
    jQuery('ul.noext-menu').removeClass('noext-menu-hidden').each( function () {
        noext.Menu.menuInstances.push( new noext.Menu( jQuery(this) ) );
    });
});