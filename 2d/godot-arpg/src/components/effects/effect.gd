extends AnimatedSprite2D

# Called when the node enters the scene tree for the first time.
func _ready():
	connect("animation_finished",_on_animation_finished)
	play("animate")

func _on_animation_finished():
	queue_free()
