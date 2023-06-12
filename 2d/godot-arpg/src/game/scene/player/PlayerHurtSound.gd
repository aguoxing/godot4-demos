extends AudioStreamPlayer

func _ready():
	connect("finished", queue_free)
