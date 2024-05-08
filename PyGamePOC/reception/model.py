
class Reception(object):
    introduction = ""
    introduction_displayed = False
    options = {}

    def __init__(self):
        self.introduction = "This is a cool game!"
        self.introduction_displayed = False
        self.options = (
            "New Game",
            "Load Game",
            "Quit Game",
        )

