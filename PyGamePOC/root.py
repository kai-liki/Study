import pygame

from common.control import FONT_NAME, Controller, Scene, Button, Panel, VisibleControl, Label, Animation, AttributeLabel
import common.edu as edu
from common.game import GameController
from common.resource import ImageResource, AnimationResource


DEFAULT_SAVE='./SAVE0.DAT'


class CookController(Controller):
    def __init__(self, world: edu.World, root_controller):
        super().__init__()
        self.world = world
        self.root_controller = root_controller

    def process(self, **kwargs):
        event_name = kwargs['event_name']
        if event_name == 'mouse_click':
            self.on_cook()

    def on_cook(self):
        self.root_controller.try_trigger_event(edu.EVENT_ACTION_BEGIN, edu.STATE_ACTION_BEGIN)


class NextController(Controller):
    def __init__(self, world: edu.World):
        super().__init__()
        self.world = world

    def process(self, **kwargs):
        event_name = kwargs['event_name']
        if event_name == 'mouse_click':
            self.on_next_cycle()

    def on_next_cycle(self):
        self.world.status = edu.STATE_CYCLE_END


class LeaveController(Controller):
    def __init__(self, world: edu.World):
        super().__init__()
        self.world = world

    def process(self, **kwargs):
        event_name = kwargs['event_name']
        if event_name == 'mouse_click':
            self.on_leave()

    def on_leave(self):
        self.world.save(DEFAULT_SAVE)

        self.world.status = edu.STATE_IDLE
        self.game.current_controller = self.get_game().new_controller('RECEPTION')


class ActionController(Controller):
    def __init__(self, world: edu.World):
        super().__init__()
        self.world = world

    def process(self, **kwargs):
        event_name = kwargs['event_name']
        if event_name == 'mouse_click':
            self.world.status = edu.STATE_ACTION_END


class EventDialogController(Controller):
    def __init__(self, world: edu.World):
        super().__init__()
        self.world = world

    def process(self, **kwargs):
        event_name = kwargs['event_name']
        if event_name == 'mouse_click':
            print('Click event dialog panel')


class EventDialogOptionController(Controller):
    def __init__(self, world: edu.World, option_value: int, root_controller):
        super().__init__()
        self.world = world
        self.option_value = option_value
        self.root_controller = root_controller

    def process(self, **kwargs):
        event_name = kwargs['event_name']
        if event_name == 'mouse_click':
            print('Click event dialog option', self.option_value)
            assert self.root_controller.current_event is not None
            event = self.root_controller.current_event
            conversation = event.get_conversation()

            conversation.next_dialog(self.world, self.option_value)
            self.root_controller.build_event_scene(event)


class POCRootController(Controller):
    def load_ui_resource(self):
        self.ui_resources = {
            'LOGO': ImageResource('LOGO', 'resources/imgs/logo.gif', (0, 0), (100, 62)),
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
            'OPTION_DEFAULT_DOWN': ImageResource('OPTION_DEFAULT_DOWN', 'resources/imgs/OptionButton.gif', (0, 40), (280, 20)),
            'JUMP_BALL': AnimationResource('JUMP_BALL', 'resources/imgs/JumpBall.png', (0, 0), (16, 16), 12),
        }

    def _init_scene(self):
        # Build START scene
        self.scene_start.register_controller(self)
        self.scene_start.set_size(320, 240)
        self.scene_start.set_bg_color((255, 255, 255))

        logo = Panel('PANEL_LOGO')
        logo.set_size(100, 62)
        logo.set_position(100, 90)
        logo.set_bg_color((255, 255, 255))
        logo.set_background_image(self.ui_resources['LOGO'])
        self.scene_start.add_control(logo)

        # Build INTRO scene
        self.scene_intro.register_controller(self)
        self.scene_intro.set_size(320, 240)
        self.scene_intro.set_bg_color((0, 0, 0))
        # self.scene_intro.set_background_image(self.ui_resources['SCENE_INTRO'])

        intro_label = Label('LABEL_INTRO')
        intro_label.set_labels(['这是一个POC。', '欢迎建议！'])
        intro_label.set_label_color((255, 255, 255))
        intro_label.set_size(100, 100)
        intro_label.set_position(100, 90)
        intro_label.set_font(pygame.font.SysFont(FONT_NAME, 10))
        self.scene_intro.add_control(intro_label)

        # Build MAIN scene
        self.scene_main.register_controller(self)
        self.scene_main.set_size(320, 240)
        self.scene_main.set_background_image(self.ui_resources['SCENE_MAIN'])

        player_attribute_panel = Panel('PANEL_PLAYER_ATTRIB')
        player_attribute_panel.set_size(100, 80)
        player_attribute_panel.set_position(1, 1)
        player_attribute_panel.set_bg_color((0, 0, 255))
        self.scene_main.add_control(player_attribute_panel)

        player_attrib_strength = AttributeLabel('ATTRIB_LABEL_PLAYER_STRENGTH', self.world.characters['PLAYER'], '体力', 'strength')
        player_attrib_strength.set_size(80, 20)
        player_attrib_strength.set_position(10, 2)
        player_attrib_strength.set_bg_color((255, 255, 255))
        player_attrib_strength.set_title_font(pygame.font.SysFont(FONT_NAME, 10))
        player_attrib_strength.set_title_color((255, 0, 0))
        player_attrib_strength.set_attribute_font(pygame.font.SysFont(FONT_NAME, 10))
        player_attrib_strength.set_attribute_color((0, 0, 0))

        player_attrib_cooking_skill = AttributeLabel('ATTRIB_LABEL_PLAYER_COOKING', self.world.characters['PLAYER'], '厨艺', 'cooking_skill')
        player_attrib_cooking_skill.set_size(80, 20)
        player_attrib_cooking_skill.set_position(10, 25)
        player_attrib_cooking_skill.set_bg_color((255, 255, 255))
        player_attrib_cooking_skill.set_title_font(pygame.font.SysFont(FONT_NAME, 10))
        player_attrib_cooking_skill.set_title_color((255, 0, 0))
        player_attrib_cooking_skill.set_attribute_font(pygame.font.SysFont(FONT_NAME, 10))
        player_attrib_cooking_skill.set_attribute_color((0, 0, 0))

        player_attrib_luckiness = AttributeLabel('ATTRIB_LABEL_PLAYER_LUCKINESS', self.world.characters['PLAYER'], '运气', 'luckiness')
        player_attrib_luckiness.set_size(80, 20)
        player_attrib_luckiness.set_position(10, 48)
        player_attrib_luckiness.set_bg_color((255, 255, 255))
        player_attrib_luckiness.set_title_font(pygame.font.SysFont(FONT_NAME, 10))
        player_attrib_luckiness.set_title_color((255, 0, 0))
        player_attrib_luckiness.set_attribute_font(pygame.font.SysFont(FONT_NAME, 10))
        player_attrib_luckiness.set_attribute_color((0, 0, 0))

        player_attribute_panel.add_controls([player_attrib_strength, player_attrib_cooking_skill, player_attrib_luckiness])

        # self.label_calendar.register_controller(Controller())
        self.label_calendar.set_size(40, 20)
        self.label_calendar.set_position(260, 20)
        self.label_calendar.set_labels([''])
        self.label_calendar.set_label_color((255, 255, 255))
        self.label_calendar.set_font(pygame.font.SysFont(FONT_NAME, 18))
        self.scene_main.add_control(self.label_calendar)

        self.button_cook.set_size(50, 50)
        self.button_cook.set_position(50, 190)
        self.button_cook.set_label('Cook')
        self.button_cook.set_button_images((self.ui_resources['ACTION_SAMPLE_NORMAL'],
                                            self.ui_resources['ACTION_SAMPLE_OVER'],
                                            self.ui_resources['ACTION_SAMPLE_DOWN']))
        self.cook_controller = CookController(self.world, self)
        self.cook_controller.set_game(self.game)
        self.button_cook.register_controller(self.cook_controller)
        self.scene_main.add_control(self.button_cook)

        self.button_next.set_size(50, 50)
        self.button_next.set_position(190, 190)
        self.button_next.set_label('Next')
        self.button_next.set_button_images((self.ui_resources['ACTION_SAMPLE_NORMAL'],
                                            self.ui_resources['ACTION_SAMPLE_OVER'],
                                            self.ui_resources['ACTION_SAMPLE_DOWN']))
        self.next_controller = NextController(self.world)
        self.next_controller.set_game(self.game)
        self.button_next.register_controller(self.next_controller)
        self.scene_main.add_control(self.button_next)

        self.button_leave.set_size(50, 50)
        self.button_leave.set_position(120, 190)
        self.button_leave.set_label('Leave')
        self.button_leave.set_button_images((self.ui_resources['ACTION_SAMPLE_NORMAL'],
                                            self.ui_resources['ACTION_SAMPLE_OVER'],
                                            self.ui_resources['ACTION_SAMPLE_DOWN']))
        self.leave_controller = LeaveController(self.world)
        self.leave_controller.set_game(self.game)
        self.button_leave.register_controller(self.leave_controller)
        self.scene_main.add_control(self.button_leave)

        # Build ENDING scene
        self.scene_ending.register_controller(self)
        self.scene_ending.set_size(320, 240)
        self.scene_ending.set_background_image(self.ui_resources['SCENE_ENDING'])

        # Build ACTION scene
        self.scene_action.register_controller(self)
        self.scene_action.set_size(320, 240)
        self.scene_action.set_background_image(self.ui_resources['SCENE_KITCHEN'])

        self.panel_action.set_size(200, 150)
        self.panel_action.set_position(60, 45)
        self.panel_action.set_bg_color((255, 255, 255))
        self.panel_action.set_background_image(self.ui_resources['PANEL_ACTION'])
        self.action_panel_controller = ActionController(self.world)
        self.panel_action.register_controller(self.action_panel_controller)
        self.scene_action.add_control(self.panel_action)

        # Build EVENT scene
        self.scene_event.register_controller(self)
        self.scene_event.set_size(320, 240)
        self.scene_event.set_bg_color((0, 0, 0))

        self.panel_event_dialog.set_size(300, 120)
        self.panel_event_dialog.set_position(10, 110)
        self.panel_event_dialog.set_visible(False)
        # self.panel_event_dialog.set_layer(1)
        self.event_dialog_controller = EventDialogController(self.world)
        self.panel_event_dialog.register_controller(self.event_dialog_controller)
        self.scene_event.add_control(self.panel_event_dialog)

        self.panel_event_dialog_label.set_size(280, 40)
        self.panel_event_dialog_label.set_position(10, 2)
        self.panel_event_dialog_label.set_font(pygame.font.SysFont(FONT_NAME, 12))
        self.panel_event_dialog_label.set_label_space(15)
        self.panel_event_dialog_label.set_visible(False)
        self.panel_event_dialog_label.set_layer(1)
        self.panel_event_dialog.add_control(self.panel_event_dialog_label)

        jump_ball = Animation('JUMP_BALL', self.ui_resources['JUMP_BALL'])
        jump_ball.set_size(16, 16)
        jump_ball.set_position(280, 100)
        jump_ball.set_layer(3)
        self.panel_event_dialog.add_control(jump_ball)

        option_idx = 0
        for button in self.buttons_dialog_option:
            option_button_controller = EventDialogOptionController(self.world, option_idx, self)
            button.set_size(280, 20)
            button.set_position(10, 50 + option_idx * 22)
            button.set_font(pygame.font.SysFont(FONT_NAME, 12))
            button.set_layer(3)
            button.set_label('Option')
            button.set_button_images((self.ui_resources['OPTION_DEFAULT_NORMAL'],
                                      self.ui_resources['OPTION_DEFAULT_OVER'],
                                      self.ui_resources['OPTION_DEFAULT_DOWN']))
            button.set_visible(False)
            button.register_controller(option_button_controller)
            self.panel_event_dialog.add_control(button)
            option_idx = option_idx + 1

        self.panel_event_dialog_char.set_size(80, 100)
        self.panel_event_dialog_char.set_position(250, 50)
        self.panel_event_dialog_char.set_visible(False)
        self.panel_event_dialog_char.set_layer(4)
        self.scene_event.add_control(self.panel_event_dialog_char)

    def __init__(self):
        self.scene_start = Scene('SCENE_START')
        self.scene_intro = Scene('SCENE_INTRO')
        self.scene_main = Scene('SCENE_MAIN')
        self.scene_action = Scene('SCENE_ACTION')
        self.scene_ending = Scene('SCENE_ENDING')
        self.scene_event = Scene('SCENE_EVENT')

        self.panel_calendar = Panel('PANEL_CALENDAR')
        self.panel_action = Panel('PANEL_ACTION')
        self.panel_event_dialog = Panel('PANEL_EVENT_DIALOG')
        self.panel_event_dialog_label = Label('LABEL_EVENT_DIALOG_LABEL')
        self.panel_event_dialog_char = Panel('PANEL_EVENT_DIALOG_CHAR')

        self.button_leave = Button('FUNC_BTN_LEAVE')
        self.button_next = Button('FUNC_BTN_NEXT')

        self.button_cook = Button('ACTION_BTN_COOK')

        self.buttons_dialog_option = [
            Button('OPTION_BTN_1'),
            Button('OPTION_BTN_2'),
            Button('OPTION_BTN_3'),
        ]

        self.leave_controller: LeaveController
        self.next_controller: NextController

        self.cook_controller: CookController

        self.event_dialog_controller: EventDialogController

        self.label_calendar = Label('LABEL_CALENDAR')

        self.ui_resources: dict

        self.load_ui_resource()

    def initialize(self, saved_data=None):
        if saved_data is None:
            world, almanac = edu.build_POC(self.ui_resources)
        else:
            _, almanac = edu.build_POC(self.ui_resources)
            world = saved_data
        self.world = world
        self.almanac = almanac
        self._init_scene()

        # Init event
        self.current_event = None

    def build_event_scene(self, event: edu.Event):
        conversation = event.get_conversation()
        self.scene_event.set_background_image(conversation.get_scene_resource())
        dialog = conversation.get_current_dialog()
        if dialog is None:
            self.panel_event_dialog.set_visible(False)
            self.panel_event_dialog_char.set_visible(False)
            self.panel_event_dialog_label.set_visible(False)
        else:
            self.panel_event_dialog.set_visible(True)
            self.panel_event_dialog_char.set_visible(False)
            self.panel_event_dialog_label.set_visible(True)
            if type(dialog) is dict:
                for button in self.buttons_dialog_option:
                    button.set_visible(False)
                if 'speaker' in dialog and dialog['speaker'] != 'PLAYER':
                    assert self.world.characters[dialog['speaker']].appearance['DEFAULT'] is not None
                    self.panel_event_dialog_char.set_background_image(self.world.characters[dialog['speaker']].appearance['DEFAULT'])
                    self.panel_event_dialog_char.set_visible(True)
                if 'speaker' in dialog:
                    labels = [
                        self.world.characters[dialog['speaker']].name,
                        dialog['content']
                    ]
                else:
                    labels = [
                        dialog['content']
                    ]
                self.panel_event_dialog_label.set_labels(labels)
            else:
                option_idx = 0
                for option_dialog in dialog:
                    button = self.buttons_dialog_option[option_idx]
                    if option_idx < len(dialog):
                        button.set_visible(True)
                        button.set_label(option_dialog['content'])
                    else:
                        button.set_visible(False)
                    option_idx = option_idx + 1

    def set_game(self, game: GameController):
        super().set_game(game)

    def get_scene(self):
        if self.world.status is edu.STATE_START:
            self.start_tick()
            return self.scene_start
        elif self.world.status is edu.STATE_INTRO:
            return self.scene_intro
        elif self.world.status is edu.STATE_CYCLE_BEGIN:
            self.on_cycle_begin()
            return self.scene_main
        elif self.world.status is edu.STATE_WAIT_ACTION:
            return self.scene_main
        elif self.world.status is edu.STATE_CYCLE_END:
            self.on_cycle_end()
            return self.scene_main
        elif self.world.status is edu.STATE_ACTION_BEGIN:
            return self.scene_action
        elif self.world.status is edu.STATE_ACTION_END:
            self.on_action_end()
            return self.scene_action
        elif self.world.status is edu.STATE_EVENT:
            assert self.current_event is not None
            return self.scene_event
        elif self.world.status is edu.STATE_IDLE:
            return self.scene_ending

    def process(self, **kwargs):
        event_name = kwargs['event_name']
        if event_name == 'on_key_down':
            key = kwargs['key']
            self.on_key_down(key)
        # elif event_name == 'mouse_click':
        #     self.on_mouse_click()

    def try_trigger_event(self, event_type, next_state):
        # Event
        event = self.almanac.divine(self.world, event_type)
        if event is None:
            self.world.status = next_state
        else:
            self.world.status = edu.STATE_EVENT
            self.world.event_type = event_type
            self.current_event = event
            conversation = event.get_conversation()
            conversation.reset_dialog()
            conversation.next_dialog(self.world)
            self.build_event_scene(self.current_event)

    def on_cycle_begin(self):
        player = self.world.characters['PLAYER']
        player.strength = player.max_strength
        self.button_cook.set_is_enabled(True)
        current = self.world.calendar.next()
        if current is not None:
            self.label_calendar.set_labels([current])

        # Event
        self.try_trigger_event(edu.EVENT_CYCLE_BEGIN, edu.STATE_WAIT_ACTION)
        # event = self.almanac.divine(self.world, edu.EVENT_CYCLE_BEGIN)
        # if event is None:
        #     self.world.status = edu.STATE_WAIT_ACTION
        # else:
        #     self.world.status = edu.STATE_EVENT
        #     self.world.event_type = edu.EVENT_CYCLE_BEGIN
        #     self.current_event = event
        #     conversation = event.get_conversation()
        #     conversation.next_dialog(self.world)
        #     self.build_event_scene(self.current_event)

    def on_cycle_end(self):
        self.world.status = edu.STAGE_CYCLE_BEGIN

    def on_action_end(self):
        self.try_trigger_event(edu.EVENT_ACTION_END, edu.STATE_WAIT_ACTION)
        player = self.world.characters['PLAYER']
        player.strength = 0 if player.strength < 10 else player.strength - 10
        self.button_cook.set_is_enabled(player.strength >= 10)
        self.world.status = edu.STATE_WAIT_ACTION

    def on_key_down(self, key):
        if self.world.status is edu.STATE_INTRO:
            self.world.status = edu.STATE_CYCLE_BEGIN
            # TOTO: Add event
        elif self.world.status is edu.STATE_ACTION_BEGIN:
            self.world.status = edu.STATE_ACTION_END
        elif self.world.status is edu.STATE_EVENT:
            assert self.current_event is not None
            conversation = self.current_event.get_conversation()
            if not conversation.is_waiting_input():
                dialog = conversation.next_dialog(self.world)
                if dialog is not None:
                    self.build_event_scene(self.current_event)
                else:
                    self.current_event.post_event(self.world)
                    if self.world.event_type is edu.EVENT_CYCLE_BEGIN:
                        self.world.status = edu.STATE_WAIT_ACTION
                    elif self.world.event_type is edu.EVENT_ACTION_BEGIN:
                        self.world.status = edu.STATE_ACTION_BEGIN
                    # TODO: Here to add more event redirections
        elif self.world.status is edu.STATE_IDLE:
            self.game.current_controller = self.game.new_controller('RECEPTION')

    def on_mouse_click(self):
        pass

    def start_tick(self):
        stage = self.world.stages[edu.STAGE_START]
        if stage.tick > stage.max_tick:
            self.world.status = edu.STAGE_INTRO
        else:
            stage.tick = stage.tick + 1

    def load(self, filename: str=DEFAULT_SAVE):
        return edu.load_world(filename)

