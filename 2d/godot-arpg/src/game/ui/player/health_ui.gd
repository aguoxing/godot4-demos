extends Control

var health = 100: set = set_health
var max_health = 100: set = set_max_health

@onready var hpBar = $Health
@onready var scoreLabel = $Score

func set_health(value):
	health = clamp(value, 0, max_health)
	hpBar.value = self.health

func set_max_health(value):
	max_health = max(value, 1)
	self.health = min(health, max_health)
	hpBar.max_value = self.max_health

func set_score(value):
	scoreLabel.text = str(value)
	
func _ready():
	self.max_health = PlayerStats.max_health
	self.health = PlayerStats.health
	PlayerStats.health_changed.connect(set_health)
	PlayerStats.max_health_changed.connect(set_max_health)
	PlayerStats.score_changed.connect(set_score)
