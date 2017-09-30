#!/usr/bin/env python
# -*- coding: utf-8 -*-
#
# Simple Bot to reply to Telegram messages
# This program is dedicated to the public domain under the CC0 license.
"""
This Bot uses the Updater class to handle the bot.

First, a few handler functions are defined. Then, those functions are passed to
the Dispatcher and registered at their respective places.
Then, the bot is started and runs until we press Ctrl-C on the command line.

Usage:
Basic Echobot example, repeats messages.
Press Ctrl-C on the command line or send a signal to the process to stop the
bot.
"""
from telegram import (ReplyKeyboardMarkup, ReplyKeyboardRemove)
from telegram.ext import Updater, CommandHandler, MessageHandler, Filters
import logging

#!/usr/bin/env python
import paho.mqtt.client as mqtt
import time

# Enable logging
logging.basicConfig(format='%(asctime)s - %(name)s - %(levelname)s - %(message)s',
                    level=logging.INFO)

logger = logging.getLogger(__name__)


regristrierteClients = [101142135]
bot = None
client = None
mgttServer = ""
ringTopic = "/theringbot/ring"
openingTopic = "/theringbot/opening"

# Define a few command handlers. These usually take the two arguments bot and
# update. Error handlers also receive the raised TelegramError object in error.
def start(bot, update):
    update.message.reply_text('Hi')
    regristrierteClients.append(update.message.chat_id)
    

def help(bot, update):
    update.message.reply_text('Help!')
   

def config(bot, update):
    global mqttServer
    update.message.reply_text('Config Setup started...')
    bot.send_message(chat_id=update.message.chat_id, text=str("Wie heißt der MQTT Server?"))    
    
#Fragt ob man eine Benachrichtigung bekommmen will
def status(bot, update):
    reply_keyboard = [['Ich bin zuhause', 'Ich bin unterwegs', 'Ich will nicht gestört werden']]
    update.message.reply_text('Wie ist dein aktueller Status?', reply_markup=ReplyKeyboardMarkup(reply_keyboard, one_time_keyboard=True))


#Reagieren auf Usereingabe
def write(bot, update):
    if update.message.text == 'Ich bin zuhause':
        if not update.message.chat_id in regristrierteClients: 
            regristrierteClients.append(update.message.chat_id)
        update.message.reply_text("Du wirst nun benachrichtigt wenn es klingelt")
    elif update.message.text == 'Ich bin unterwegs':
        if update.message.chat_id in regristrierteClients: 
            regristrierteClients.remove(update.message.chat_id)
        update.message.reply_text("Du wirst nun nicht mehr benachrichtigt wenn es klingelt")
    elif update.message.text == 'Ich will nicht gestört werden':
        if update.message.chat_id in regristrierteClients: 
            regristrierteClients.remove(update.message.chat_id)
        update.message.reply_text("Du wirst nun nicht mehr benachrichtigt wenn es klingelt")
    elif update.message.text == 'Öffne die Tür':
        client.publish(openingTopic, "t:" + str(update.message.chat_id) + ":" + update.message.from_user.first_name)
        print("Tür wird geöffnet") 
    elif update.message.text == 'Ich kann die Tür gerade nicht aufmachen':
        client.publish(openingTopic, "a:" + str(update.message.chat_id) + ":" + update.message.from_user.first_name)

    print(regristrierteClients)  


#Reagieren auf neue Nachricht aus MQTT
def on_message(client, userdata, msg):
    print(str(msg.payload))
    if msg.topic == ringTopic:
        ring(client, userdata, msg)
    elif msg.topic == openingTopic:
        openDoor(client, userdata, msg)


def ring(client, userdata, msg):
    global ChatNachricht
    if len(regristrierteClients) > 0:
        if str(msg.payload.decode("ascii")[0] == 'k'):
            ChatNachricht = "Es hat an der Tür geklingelt"
            for chat_id in regristrierteClients:
                bot.send_message(chat_id=chat_id, text=ChatNachricht)
                reply_keyboard = [['Öffne die Tür', 'Ich kann die Tür gerade nicht aufmachen', 'Keine Reaktion']]
                bot.send_message(chat_id=chat_id, text='Was soll ich tun?', reply_markup=ReplyKeyboardMarkup(reply_keyboard, one_time_keyboard=True))


def openDoor(client, userdata, msg):
    message = msg.payload.decode("ascii").split(':')

    if str(message[0]) == 't':
        ChatNachricht = "Die Tür wird von " + message[2] +" geöffnet"
    elif str(message[0]) == 'a':
        ChatNachricht = message[2] + " kann die Tür nicht öffnen"

    for chat_id in regristrierteClients:
        if int(message[1]) == chat_id:
            bot.send_message(chat_id=chat_id, text="Nachricht wurde an die anderen Mitbewohner gesendet")
        else:
            bot.send_message(chat_id=chat_id, text=ChatNachricht)
        

def error(bot, update, error):
    logger.warn('Update "%s" caused error "%s"' % (update, error))


def on_connect(client, userdata, flags, rc):
    print("Connected with result code " + str(rc))

    client.subscribe(ringTopic)
    client.subscribe(openingTopic)     
               

def main():
    global bot, client
    # Create the EventHandler and pass it your bot's token.
    updater = Updater("479238432:AAH16mDXaHlnqa0lwog398OePSvL-ouEdb0")
    bot = updater.bot
    # Get the dispatcher to register handlers
    dp = updater.dispatcher

    # on different commands - answer in Telegram
    dp.add_handler(CommandHandler("start",  start))
    dp.add_handler(CommandHandler("help",   help))
    dp.add_handler(CommandHandler("config", config))
    dp.add_handler(CommandHandler("status", status))
    dp.add_handler(MessageHandler(Filters.text, write))

    # log all errors
    dp.add_error_handler(error)

    client = mqtt.Client()
    client.on_connect = on_connect
    client.on_message = on_message
    
    client.connect("172.16.0.1", 1883, 60)

    # Start the Bot
    updater.start_polling()

    # Run the bot until you press Ctrl-C or the process receives SIGINT,
    # SIGTERM or SIGABRT. This should be used most of the time, since
    # start_polling() is non-blocking and will stop the bot gracefully.
    #
    #updater.idle()
    client.loop_forever()


if __name__ == '__main__':
    main()
