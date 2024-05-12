import pygame

from PyGamePOC.common.control import Controller, Scene
from PyGamePOC.common.resource import ImageResource


EVENT_CYCLE_BEGIN = 0
EVENT_CYCLE_END = 1
EVENT_CYCLE_BETWEEN = 2
EVENT_ACTION_BEGIN = 3
EVENT_ACTION_END = 4
EVENT_KHALAS = 5

STATUS_START = 0
STATUS_INTRO = 1
STATUS_CYCLE_BEGIN = 2
STATUS_CYCLE_END = 3
STATUS_CYCLE_BETWEEN = 4
STATUS_WAIT_ACTION = 5
STATUS_ACTION_BEGIN = 6
STATUS_ACTION_END = 7
STATUS_KHALAS = 8


class Character:
    def __init__(self, appearance: dict):
        self._attribute = {}
        self.appearance = appearance

    def __getattr__(self, item):
        return self._attribute[item]

    def __setattr__(self, key, value):
        self._attribute[key] = value


class Calendar:
    def __init__(self):
        self.rounds = []
        self.index = -1

    def append_round(self, calendar_round):
        self.rounds.append(calendar_round)

    def append_rounds(self, rounds):
        self.rounds.extend(rounds)

    def __next__(self):
        if self.index + 1 < len(self.rounds):
            self.index = self.index + 1
            return self.rounds[self.index]
        else:
            return None


class World:
    _attribute = {}

    def __init__(self, characters: dict, scenes: dict, calendar: Calendar):
        self.characters = characters
        self.scenes = scenes
        self.calendar = calendar
        self.status = STATUS_START

    def __getattr__(self, item):
        return self._attribute[item]

    def __setattr__(self, key, value):
        self._attribute[key] = value

    def applicable_actions(self):
        return ()

    def serialize(self):
        pass

    def deserialize(self):
        pass


class Effective:
    effects = {}

    def __init__(self, effectiveness):
        assert callable(effectiveness)
        self.effectiveness = effectiveness

    def effect(self, world: World):
        self.effectiveness(world)


class Dialog(Effective):
    scene_resource: ImageResource = None
    dialog_list: list
    dialog_dict: dict
    dialog_index: int

    # Example:
    # dialog_flow =
    # [
    #   {speaker : 'player', content : 'Good morning!'},
    #   {speaker : 'girl', content : 'Morning! How are you?'},
    #   [
    #       {speaker : 'player', content : 'Fine, thank you. And you?'},
    #       {speaker : 'player', content : 'I'm good. How are you?'},
    #   ]
    #   {speaker : 'girl', content : 'Super good!'},
    #   {speaker : 'girl', content : 'I'm waiting for my boyfriend. Talk you later!'},
    #   [
    #       {speaker : 'player', content : 'Bye!', next : 'finish'},
    #       {speaker : 'player', content : 'Wait! He is a liar!', next : 'entry_01'},
    #   ]
    #   {id : 'entry_01', speaker : 'girl', content : 'YOU SHUT UP!'},
    #   {id : 'finish', content : 'The girl left me.'}
    # ]
    def __init__(self, scene_resource, dialog_flow, effectiveness):
        super().__init__(effectiveness)
        self.scene_resource = scene_resource
        self._build_dialogs(dialog_flow)
        self.dialog_index = -1

    def _build_dialogs(self, dialog_flow):
        self.dialog_list = []
        self.dialog_dict = {}
        idx = 0
        for dialog in dialog_flow:
            if 'id' in dialog:
                self.dialog_dict[dialog['id']] = idx
            self.dialog_list.append(dialog)
            idx = idx + 1

    def next_dialog(self, option=-1):
        if self.dialog_index + 1 < len(self.dialog_list):
            jumped = False
            if self.dialog_index != -1:
                dialog = self.dialog_list[self.dialog_index]
                if type(dialog) is dict:
                    if 'next' in dialog:
                        self.dialog_index = self.dialog_dict[dialog['next']]
                        jumped = True
                elif type(dialog) is list and option >= 0:
                    option_dialog = dialog[option]
                    if 'next' in option_dialog:
                        self.dialog_index = self.dialog_dict[option_dialog['next']]
                        jumped = True
            if not jumped:
                self.dialog_index = self.dialog_index + 1
            return self.dialog_list[self.dialog_index]
        else:
            return None


class Event:
    dialog: Dialog

    def __init__(self, dialog: Dialog, condition_func, post_func):
        assert callable(condition_func)
        self.dialog = dialog
        self.condition_func = condition_func
        self.post_func = post_func

    def match(self, world: World):
        return self.condition_func(world)

    def post_event(self, world: World):
        self.post_func(world)


class Almanac:
    event_lib = [
        {},
        {},
        {},
        {},
        {},
        {},
    ]

    def __init__(self):
        pass

    def divine(self, world: World):
        pass

    def add_event(self, event_stage: int, event_id: str, event: Event):
        self.event_lib[event_stage][event_id] = event


class UI:
    scene_intro: ImageResource
    scene_main: ImageResource
    scene_event: ImageResource
    button_actions: dict
    panel_actions: dict
    panel_message: ImageResource
    panel_dialog: ImageResource
    button_option: ImageResource
    scene_ending: ImageResource


if __name__ == "__main__":
    def effect(world: World):
        print('Heal the word!')

    dialog_flow = [
        {'speaker': 'player', 'content': 'Good morning!'},
        {'speaker': 'girl', 'content': 'Morning! How are you?'},
        [
            {'speaker': 'player', 'content': 'Fine, thank you. And you?'},
            {'speaker': 'player', 'content': 'I\'m good. How are you?'},
        ],
        {'speaker': 'girl', 'content': 'Super good!'},
        {'speaker': 'girl', 'content': 'I\'m waiting for my boyfriend. Talk you later!'},
        [
            {'speaker': 'player', 'content': 'Bye!', 'next': 'finish'},
            {'speaker': 'player', 'content': 'Wait! He is a liar!', 'next': 'entry_01'},
        ],
        {'id': 'entry_01', 'speaker': 'girl', 'content': 'YOU SHUT UP!', 'next': 'finish'},
        {'speaker': 'player', 'content': 'I don\'t have chance to speak.'},
        {'id': 'finish', 'content': 'The girl left me.'}
    ]
    event_dialog = Dialog(None, dialog_flow, effect)

    dialog = event_dialog.next_dialog()
    while dialog is not None:
        if type(dialog) is dict:
            if 'speaker' in dialog:
                print(str.format("{}: {}", dialog['speaker'], dialog['content']))
            else:
                print(str.format("{}",  dialog['content']))
            # option = input()
            dialog = event_dialog.next_dialog()
        else:
            for option_dialog in dialog:
                if 'speaker' in option_dialog:
                    print(str.format("- {}: {}", option_dialog['speaker'], option_dialog['content']))
                else:
                    print(str.format("- {}", option_dialog['content']))
            option = input()
            dialog = event_dialog.next_dialog(int(option))

    world = World({}, {}, Calendar())
    event_dialog.effect(world)

