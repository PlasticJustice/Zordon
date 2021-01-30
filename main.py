import discord

client = discord.Client()

@client.event
async def on_ready():
    print('We have logged in as {0.user}'.format(client))

@client.event
async def on_message(message):
    if message.author == client.user:
        return

    if message.content.startswith('.hello'):
        await message.channel.send('Hello!')

client.login(ODA0NzY3OTU1NzUyODQ1NDAz.YBRIkA.DEaiBc6sEfj05AX5X9-5ouSHQUY, true)
client.connect()
client.run()