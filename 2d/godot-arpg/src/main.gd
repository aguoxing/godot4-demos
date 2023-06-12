extends Control

func _on_start_pressed():
	get_tree().change_scene_to_file("res://src/game/game.tscn")

func _on_load_pressed():
	pass # Replace with function body.

func _on_setting_pressed():
	$AnimationPlayer.play("Setting")

func _on_exit_pressed():
	get_tree().quit()
