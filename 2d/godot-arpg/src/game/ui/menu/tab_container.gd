extends TabContainer

signal show_tab_menu

var is_visible =  true

# Called when the node enters the scene tree for the first time.
func _ready():
	if(is_visible):
		show()
	else:
		hide()

func show_menu():
	show()
