extends Control


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

func _on_button_pressed():
	$AnimationPlayer.play("ShowSetting")
#	await $AnimationPlayer.animation_finished

func _on_button_3_pressed():
	$AnimationPlayer.play_backwards("ShowSetting")
