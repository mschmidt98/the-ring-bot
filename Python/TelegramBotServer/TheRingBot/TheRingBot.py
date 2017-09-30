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
import base64

#!/usr/bin/env python
import paho.mqtt.client as mqtt
import time
import os

# Enable logging
logging.basicConfig(format='%(asctime)s - %(name)s - %(levelname)s - %(message)s',
                    level=logging.INFO)

logger = logging.getLogger(__name__)


regristrierteClients = []
bot = None
client = None
mgttServer = ""
ringTopic = "/theringbot/ring"
openingTopic = "/theringbot/opening"
picTopic = "/theringbot/pic"
config = {"mqttip" : "172.16.0.1", "ringchannel" : "/theringbot/ring", "statuschannel" : "/theringbot/opening", "picchannel" : "/theringbot/pic"}

# Define a few command handlers. These usually take the two arguments bot and
# update. Error handlers also receive the raised TelegramError object in error.
def start(bot, update):
    update.message.reply_text('Hi')
    regristrierteClients.append(update.message.chat_id)

    
def help(bot, update):
    update.message.reply_text('Help!')
    update.message.reply_text("/mqttip      Setzt die IP für deinen MQTT-Server fest")
    update.message.reply_text("/ringchannel     Setzt den Channel auf dem MQTT-Server für alle Infos zum Klingeln und Status zum Klingeln")
    update.message.reply_text("/statuschannel      Setzt den Channel auf dem MQTT-Server fest für alle Infos zum Status der Tür")
    update.message.reply_text("/picchannel      Setzt den Channel auf dem MQTT-Server auf dem das Bild der Kamera liegt")  
    

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
    elif "/mqttip" in update.message.text:
        mqttip(bot, update)
    elif "/ringchanel" in update.message.text:
        ringchannel(bot, update)
    elif "/statuschannel" in update.message.text:
        statuschannel(bot, update)
    elif "/picchannel" in update.message.text:
        picchannel(bot, update)

    print(regristrierteClients)  


#Reagieren auf neue Nachricht aus MQTT
def on_message(client, userdata, msg):
    print(str(msg.payload[:50]))
    if msg.topic == ringTopic:
        ring(client, userdata, msg)
    elif msg.topic == openingTopic:
        openDoor(client, userdata, msg)
    elif msg.topic == picTopic:
        sendpic(client, userdata, msg)


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
        if len(message)>1:
            ChatNachricht = "Die Tür wird von " + message[2] +" geöffnet"
        else:
            ChatNachricht = "Die Tür wird geöffnet"            
    elif str(message[0]) == 'a':
        ChatNachricht = message[2] + " kann die Tür nicht öffnen"
    elif str(message[0]) == 'o' :
        ChatNachricht = "Tür ist offen"
    elif str(message[0]) == 'z':
        ChatNachricht = "Tür ist geschlossen"


    for chat_id in regristrierteClients:
        if str(message[0]) == 't' and len(message)>1 and (message[1]) == chat_id :
            bot.send_message(chat_id=chat_id, text="Nachricht wurde an die anderen Mitbewohner gesendet")
        else:
            bot.send_message(chat_id=chat_id, text=ChatNachricht)
        

def sendpic(client, userdata, msg):
    foto = base64.b64decode(msg.payload.decode("ascii"))
    try:
        os.remove("img.jpeg")
    except:
        pass
     
    with open("img.jpeg", "wb") as fp:
        fp.write(foto)    
    for chat_id in regristrierteClients:
        fp = open("img.jpeg", "rb")
        bot.send_photo(chat_id, photo=fp, timeout=10)



def listconfig(bot, update):
    global config
    strings = (str(key) + " : " + str(value) for key,value in config.items())
    text = "\n".join(strings)
    update.message.reply_text(text)
        

def mqttip(bot, update):
    global config
    eingabe = update.message.text.split(" ")[1]
    config["mqttip"] = eingabe


def ringchannel(bot, update):
    global config
    eingabe = update.message.text.split(" ")[1]
    config["ringchannel"] = eingabe


def statuschannel(bot, update):
    global config
    eingabe = update.message.text.split(" ")[1]
    config["statuschannel"] = eingabe


def picchannel(bot, update):
    global config
    eingabe = update.message.text.split(" ")[1]
    config["picchannel"] = eingabe


def error(bot, update, error):
    logger.warn('Update "%s" caused error "%s"' % (update, error))


def on_connect(client, userdata, flags, rc):
    print("Connected with result code " + str(rc))

    client.subscribe(ringTopic)
    client.subscribe(openingTopic)    
    client.subscribe(picTopic)
               

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
    dp.add_handler(CommandHandler("status", status))
    dp.add_handler(CommandHandler("mqttip", mqttip))
    dp.add_handler(CommandHandler("ringchannel", status))
    dp.add_handler(CommandHandler("statuschannel", status))
    dp.add_handler(CommandHandler("picchannel", status))
    dp.add_handler(CommandHandler("listconfig", listconfig))
    dp.add_handler(MessageHandler(Filters.text, write))

    # log all errors
    dp.add_error_handler(error)

    client = mqtt.Client()
    client.on_connect = on_connect
    client.on_message = on_message
    
    client.connect(config["mqttip"], 1883, 60)

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
