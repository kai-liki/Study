import random

import pygame

from PyGamePOC.common.resource import ImageResource


EVENT_CYCLE_BEGIN = 0
EVENT_CYCLE_END = 1
EVENT_ACTION_BEGIN = 2
EVENT_ACTION_END = 3
EVENT_KHALAS = 4

STATE_START = 0
STATE_INTRO = 1
STATE_CYCLE_BEGIN = 2
STATE_CYCLE_END = 3
STATE_EVENT = 4
STATE_WAIT_ACTION = 5
STATE_ACTION_BEGIN = 6
STATE_ACTION_END = 7
STATE_KHALAS = 8
STATE_IDLE = 9

STAGE_START = 0
STAGE_INTRO = 1
STAGE_CYCLE_BEGIN = 2
STAGE_CYCLE_END = 3
STAGE_EVENT = 4
STAGE_ACTION_BEGIN = 5
STAGE_ACTION_END = 6
STAGE_WAIT_ACTION = 7
STAGE_KHALAS = 8
STAGE_IDLE = 9


class Character:
    _attribute = {}

    def __init__(self, appearance: dict):
        self.appearance = appearance

    def __getattr__(self, item):
        return self._attribute[item]

    def __setattr__(self, key, value):
        self._attribute[key] = value


class Stage:
    _attribute = {}

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

    def has_next(self):
        return self.index + 1 < len(self.rounds)
    def __next__(self):
        if self.index + 1 < len(self.rounds):
            self.index = self.index + 1
            return self.rounds[self.index]
        else:
            return None


class World:
    _attribute = {}

    def __init__(self, characters: dict, stages: dict, calendar: Calendar):
        self.characters = characters
        self.stages = stages
        self.calendar = calendar
        self.status = STATE_START

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


class Dialog:
    scene_resource: ImageResource = None
    dialog_list: list
    dialog_dict: dict
    dialog_index: int

    # Example:
    # dialog_flow =
    # [
    #     {'speaker': 'player', 'content': 'Good morning!'},
    #     {'speaker': 'girl', 'content': 'Morning! How are you?'},
    #     [
    #         {'speaker': 'player', 'content': 'Fine, thank you. And you?', 'oid': 'd1_o1'},
    #         {'speaker': 'player', 'content': 'I\'m good. How are you?', 'oid': 'd1_o2'},
    #     ],
    #     {'speaker': 'girl', 'content': 'Super good!'},
    #     {'speaker': 'girl', 'content': 'I\'m waiting for my boyfriend. Talk you later!'},
    #     [
    #         {'speaker': 'player', 'content': 'Bye!', 'next': 'finish', 'oid': 'd2_o1'},
    #         {'speaker': 'player', 'content': 'Wait! He is a liar!', 'next': 'entry_01', 'oid': 'd2_o2'},
    #     ],
    #     {'id': 'entry_01', 'speaker': 'girl', 'content': 'YOU SHUT UP!', 'next': 'finish'},
    #     {'speaker': 'player', 'content': 'I don\'t have chance to speak.'},
    #     {'id': 'finish', 'content': 'The girl left me.'}
    # ]
    def __init__(self, scene_resource, dialog_flow, score):
        assert callable(score)
        self.score = score
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

    def next_dialog(self, world: World, option=-1):
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
                    if 'oid' in option_dialog:
                        self.score(oid=option_dialog['oid'], world=world)
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

    def divine(self, world: World, event_stage: int):
        pass

    def add_event(self, event_stage: int, event_id: str, event: Event):
        self.event_lib[event_stage][event_id] = event


def test_dialog():
    event_stage = Stage()
    event_stage.score = 0
    world = World({}, {STAGE_EVENT: event_stage}, Calendar())

    def score(oid: str, world: World):
        event_stage = world.stages[STAGE_EVENT]
        if oid in ['d1_o1', 'd1_o2']:
            event_stage.score = event_stage.score + 50

        if oid == 'd2_o1':
            event_stage.score = event_stage.score + 50
        elif oid == 'd2_o2':
            event_stage.score = event_stage.score - 40

    dialog_flow = [
        {'speaker': 'player', 'content': 'Good morning!'},
        {'speaker': 'girl', 'content': 'Morning! How are you?'},
        [
            {'speaker': 'player', 'content': 'Fine, thank you. And you?', 'oid': 'd1_o1'},
            {'speaker': 'player', 'content': 'I\'m good. How are you?', 'oid': 'd1_o2'},
        ],
        {'speaker': 'girl', 'content': 'Super good!'},
        {'speaker': 'girl', 'content': 'I\'m waiting for my boyfriend. Talk you later!'},
        [
            {'speaker': 'player', 'content': 'Bye!', 'next': 'finish', 'oid': 'd2_o1'},
            {'speaker': 'player', 'content': 'Wait! He is a liar!', 'next': 'entry_01', 'oid': 'd2_o2'},
        ],
        {'id': 'entry_01', 'speaker': 'girl', 'content': 'YOU SHUT UP!', 'next': 'finish'},
        {'speaker': 'player', 'content': 'I don\'t have chance to speak.'},
        {'id': 'finish', 'content': 'The girl left me.'}
    ]
    event_dialog = Dialog(None, dialog_flow, score)

    dialog = event_dialog.next_dialog(world)
    while dialog is not None:
        if type(dialog) is dict:
            if 'speaker' in dialog:
                print(str.format("{}: {}", dialog['speaker'], dialog['content']))
            else:
                print(str.format("{}",  dialog['content']))
            # option = input()
            dialog = event_dialog.next_dialog(world)
        else:
            for option_dialog in dialog:
                if 'speaker' in option_dialog:
                    print(str.format("- {}: {}", option_dialog['speaker'], option_dialog['content']))
                else:
                    print(str.format("- {}", option_dialog['content']))
            option = input()
            dialog = event_dialog.next_dialog(world, int(option))

    print(event_stage.score)
    print(world.stages[STAGE_EVENT].score)


def build_POC():
    # Init resources
    ui_resources = {
        'CHAR_WIFE_NORMAL': ImageResource('CHAR_WIFE_NORMAL', 'resources/imgs/NPC01.gif', (76, 0), (70, 95)),
        'CHAR_WIFE_ANGRY': ImageResource('CHAR_WIFE_ANGRY', 'resources/imgs/NPC01.gif', (1, 0), (70, 95)),
        'SCENE_INTRO': ImageResource('SCENE_INTRO', 'resources/imgs/Livingroom.gif', (0, 0), (320, 240)),
        'SCENE_MAIN': ImageResource('SCENE_MAIN', 'resources/imgs/Livingroom.gif', (0, 0), (320, 240)),
        'SCENE_ACTION': ImageResource('SCENE_ACTION', 'resources/imgs/Livingroom.gif', (0, 0), (320, 240)),
        'SCENE_ENDING': ImageResource('SCENE_ENDING', 'resources/imgs/Livingroom.gif', (0, 0), (320, 240)),
        'SCENE_KITCHEN': ImageResource('SCENE_KITCHEN', 'resources/imgs/Kitchen.gif', (0, 0), (320, 240)),
        'PANEL_ACTION': ImageResource('PANEL_ACTION', 'resources/imgs/ActionPanel.gif', (0, 0), (200, 150)),
        'PANEL_MESSAGE': ImageResource('PANEL_MESSAGE', 'resources/imgs/MessagePanel.gif', (0, 0), (150, 150)),
        'PANEL_DIALOG': ImageResource('PANEL_DIALOG', 'resources/imgs/DialogPanel.gif', (0, 0), (320, 120)),
        'ACTION_SAMPLE_NORMAL': ImageResource('ACTION_SAMPLE_NORMAL', 'resources/imgs/ActionButton.gif', (0, 0), (50, 50)),
        'ACTION_SAMPLE_OVER': ImageResource('ACTION_SAMPLE_OVER', 'resources/imgs/ActionButton.gif', (50, 0), (50, 50)),
        'ACTION_SAMPLE_DOWN': ImageResource('ACTION_SAMPLE_DOWN', 'resources/imgs/ActionButton.gif', (100, 0), (50, 50)),
        'OPTION_DEFAULT_NORMAL': ImageResource('OPTION_DEFAULT_NORMAL', 'resources/imgs/OptionButton.gif', (0, 0), (280, 20)),
        'OPTION_DEFAULT_OVER': ImageResource('OPTION_DEFAULT_OVER', 'resources/imgs/OptionButton.gif', (0, 20), (280, 20)),
        'OPTION_DEFAULT_DOWN': ImageResource('OPTION_DEFAULT_DOWN', 'resources/imgs/OptionButton.gif', (0, 40), (280, 20))
    }

    # Build the calendar
    calendar_week = Calendar()
    calendar_week.append_rounds(('MON', 'TUE', 'WED', 'THU', 'FRI', 'SAT', 'SUN'))

    # Build characters
    character_player = Character({})
    character_player.name = 'KK'
    character_player.cooking_skill = 0
    character_player.luckiness = 100
    character_player.strength = 100
    character_player.max_strength = 100

    character_wife = Character(
        {'NORMAL': ui_resources['CHAR_WIFE_NORMAL'],
         'ANGRY': ui_resources['CHAR_WIFE_ANGRY']}
    )
    character_wife.name = 'MM'
    character_wife.love = 50

    # Build stages
    stage_start = Stage()
    stage_start.tick = 0
    stage_start.max_tick = 600

    stage_intro = Stage()
    stage_intro.introduction = 'This is a game of life.'

    stage_cycle_begin = Stage()
    stage_cycle_begin.index = 0
    stage_cycle_begin.weather = 'Sunny'

    stage_event = Stage()
    stage_event.event_type = None
    stage_event.event_id = None
    stage_event.score = 0
    stage_event.threshold = 50

    stage_wait_action = Stage()
    stage_wait_action.ids = ['COOK', 'NEXT', 'LEAVE']
    stage_wait_action.is_enabled = [True, True, True]

    stage_action_begin = Stage()
    stage_action_begin.action_id = None
    stage_action_begin.is_first_cook = True

    stage_action_end = Stage()
    stage_action_end.action_id = None

    stage_cycle_end = Stage()

    stage_khalas = Stage()
    stage_khalas.final_score = 0

    stage_idle = Stage()
    stage_idle.description = 'Have a good weekend!'

    stages = {
        STAGE_START: stage_start,
        STAGE_INTRO: stage_intro,
        STAGE_CYCLE_BEGIN: stage_cycle_begin,
        STAGE_CYCLE_END: stage_cycle_end,
        STAGE_EVENT: stage_event,
        STAGE_ACTION_BEGIN: stage_action_begin,
        STAGE_ACTION_END: stage_action_end,
        STAGE_WAIT_ACTION: stage_wait_action,
        STAGE_KHALAS: stage_khalas,
        STAGE_IDLE: stage_idle
    }

    # Build world
    world = World(characters={'PLAYER': character_player, 'WIFE': character_wife}, stages=stages, calendar=calendar_week)

    def no_score(oid, world):
        pass

    def no_post(world):
        pass

    # Build cycle begin events: sunny event
    def sunny_condition(world: World):
        stage = world.stages[STAGE_CYCLE_BEGIN]
        return stage.index % 2 == 0

    def sunny_post(world: World):
        stage = world.stages[STAGE_CYCLE_BEGIN]
        stage.weather = 'Sunny'
        player = world.characters['PLAYER']
        player.luckiness = 100

    sunny_dialog_flow = [
        {'content': 'It''s a sunny day.'}
    ]

    sunny_dialog = Dialog(ui_resources['SCENE_MAIN'], sunny_dialog_flow, no_score)
    sunny_event = Event(sunny_dialog, sunny_condition, sunny_post)

    # Build cycle begin events: raining event
    def raining_condition(world: World):
        stage = world.stages[STAGE_CYCLE_BEGIN]
        return stage.index % 2 != 0

    def raining_post(world: World):
        stage = world.stages[STAGE_CYCLE_BEGIN]
        stage.weather = 'Raining'
        player = world.characters['PLAYER']
        player.luckiness = 50

    raining_dialog_flow = [
        {'content': 'It''s a raining day.'}
    ]

    raining_dialog = Dialog(ui_resources['SCENE_MAIN'], raining_dialog_flow, no_score)
    raining_event = Event(raining_dialog, raining_condition, raining_post)

    # Build cycle end events: teach event
    def teach_condition(world: World):
        return random.randint(0, 2) == 1

    def teach_post(world: World):
        player = world.characters['PLAYER']
        player.cooking_skill = player.cooking_skill + 5
        wife = world.characters['WIFE']
        wife.love = wife.love + world.stages[STAGE_EVENT].score

    teach_dialog_flow = [
        {'speaker': 'PLAYER', 'content': 'Cooking is difficult!'},
        {'speaker': 'WIFE', 'content': 'Come on! Let me give you a lesson.'},
        [
            {'speaker': 'PLAYER', 'content': 'You are so sweet!', 'oid': 'd1_agree'},
            {'speaker': 'PLAYER', 'content': 'I''m not interested.', 'next': 'FINISH_NEGATIVE', 'oid': 'd1_reject'},
        ],
        {'speaker': 'WIFE', 'content': 'You put too much salt!'},
        {'speaker': 'WIFE', 'content': 'Put less next time. It will be better.'},
        {'speaker': 'PLAYER', 'content': 'Thank you my darling!'},
        {'speaker': 'PLAYER', 'content': 'I''ll follow your instruction next time.', 'next': 'finish'},
        {'id': 'FINISH_POSITIVE', 'speaker': 'WIFE', 'content': 'Me Me Da!'},
        {'id': 'FINISH_NEGATIVE', 'content': 'She leave me alone.'}
    ]

    def teach_score(oid: str, world: World):
        event_stage = world.stages[STAGE_EVENT]
        if oid == 'd1_agree':
            event_stage.score = 50
        elif oid == 'd1_reject':
            event_stage.score = -50

    teach_dialog = Dialog(ui_resources['SCENE_KITCHEN'], teach_dialog_flow, teach_score)
    teach_event = Event(teach_dialog, teach_condition, teach_post)

    # Build action begin events: first cook
    def first_cook_condition(world: World):
        return world.stages[STAGE_ACTION_BEGIN].is_first_cook

    def first_cook_post(world: World):
        world.characters['PLAYER'].cook_skill = world.characters['PLAYER'].cook_skill + world.stages[STAGE_EVENT].score
        world.stages[STAGE_ACTION_BEGIN].is_first_cook = False

    first_cook_dialog_flow = [
        {'speaker': 'WIFE', 'content': '你会做饭吗？'},
        [
            {'speaker': 'PLAYER', 'content': '我会呀。', 'next': 'FINISH'},
            {'speaker': 'PLAYER', 'content': '我不会。。。教教我吧。', 'next': 'TEACH_START', 'oid': 'd1_teach'},
        ],
        {'id': 'TEACH_START', 'speaker': 'WIFE', 'content': '先洗菜。'},
        {'speaker': 'WIFE', 'content': '再热油。'},
        {'speaker': 'WIFE', 'content': '把菜放进去，用铲子炒。'},
        {'speaker': 'WIFE', 'content': '炒熟就好了。'},
        {'speaker': 'PLAYER', 'content': '这不是废话吗？'},
        {'id': 'FINISH', 'speaker': 'WIFE', 'content': '那你做饭去吧！'}
    ]

    def first_cook_score(oid: str, world: World):
        event_stage = world.stages[STAGE_EVENT]
        if oid == 'd1_teach':
            event_stage.score = 10

    first_cook_dialog = Dialog(ui_resources['SCENE_KITCHEN'], first_cook_dialog_flow, first_cook_score)
    first_cook_event = Event(first_cook_dialog, first_cook_condition, first_cook_post)

    # Build action begin events: random chat
    def random_chat_condition(world: World):
        return random.randint(0, 7) == 1

    random_chat_dialog_flow = [
        {'speaker': 'WIFE', 'content': '进展怎么样？'},
        {'speaker': 'PLAYER', 'content': '还行。'},
        {'speaker': 'WIFE', 'content': '加油加油！'}
    ]

    random_chat_dialog = Dialog(ui_resources['SCENE_KITCHEN'], random_chat_dialog_flow, no_score)
    random_chat_event = Event(random_chat_dialog, random_chat_condition, no_post)

    # Build action end events: love chat
    def love_chat_condition(world: World):
        return random.randint(0, 3) == 1

    love_chat_dialog_flow = [
        {'speaker': 'WIFE', 'content': '老公辛苦啦！来擦擦汗。'},
        {'speaker': 'PLAYER', 'content': '么么哒。'}
    ]

    love_chat_dialog = Dialog(ui_resources['SCENE_KITCHEN'], love_chat_dialog_flow, no_score)
    love_chat_event = Event(love_chat_dialog, love_chat_condition, no_post)

    # Build khalas events: happy khalas
    def happy_khalas_condition(world: World):
        return world.characters['PLAYER'].cooking_skill >= 100

    happy_khalas_dialog_flow = [
        {'speaker': 'WIFE', 'content': '老公好厉害！进步好大！'},
        {'speaker': 'PLAYER', 'content': '都是老婆教的好。'},
        {'speaker': 'WIFE', 'content': '今晚吃完饭早点去洗澡。'},
        {'speaker': 'PLAYER', 'content': '嘿嘿，好好好！'},
        {'speaker': 'WIFE', 'content': '以后晚饭都你来做。'},
        {'speaker': 'PLAYER', 'content': '啊？？？'},
    ]

    happy_khalas_dialog = Dialog(ui_resources['SCENE_ENDING'], happy_khalas_dialog_flow, no_score)
    happy_khalas_event = Event(happy_khalas_dialog, happy_khalas_condition, no_post)

    # Build khalas events: sad khalas
    def sad_khalas_condition(world: World):
        return world.characters['PLAYER'].cooking_skill < 100

    sad_khalas_dialog_flow = [
        {'speaker': 'WIFE', 'content': '老公好厉害！进步好大！'},
        {'speaker': 'PLAYER', 'content': '都是老婆教的好。'},
        {'speaker': 'WIFE', 'content': '今晚吃完饭早点去洗澡。'},
        {'speaker': 'PLAYER', 'content': '嘿嘿，好好好！'},
        {'speaker': 'WIFE', 'content': '以后晚饭都你来做。'},
        {'speaker': 'PLAYER', 'content': '啊？？？'},
    ]

    sad_khalas_dialog = Dialog(ui_resources['SCENE_ENDING'], sad_khalas_dialog_flow, no_score)
    sad_khalas_event = Event(sad_khalas_dialog, sad_khalas_condition, no_post)

    almanac = Almanac()
    almanac.add_event(EVENT_CYCLE_BEGIN, 'SUNNY', sunny_event)
    almanac.add_event(EVENT_CYCLE_BEGIN, 'RAINING', raining_event)
    almanac.add_event(EVENT_CYCLE_END, 'TEACH', teach_event)
    almanac.add_event(EVENT_ACTION_BEGIN, 'FIRST_COOK', first_cook_event)
    almanac.add_event(EVENT_ACTION_BEGIN, 'RANDOM_CHAT', random_chat_event)
    almanac.add_event(EVENT_ACTION_END, 'LOVE_CHAT', love_chat_event)
    almanac.add_event(EVENT_KHALAS, 'HAPPY', happy_khalas_event)
    almanac.add_event(EVENT_KHALAS, 'SAD', sad_khalas_event)

    print('Done')

    return world, almanac


if __name__ == "__main__":
    # test_dialog()
    build_POC()

