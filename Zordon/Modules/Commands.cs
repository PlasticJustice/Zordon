using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Zordon.Modules {
    // for commands to be available, and have the Context passed to them, we must inherit ModuleBase
    public class ExampleCommands : ModuleBase {
        [Command("help")]
        public async Task HelpMsg([Remainder] string cmd) {
            var sb = new StringBuilder();
            var embed = new EmbedBuilder();
            if (cmd == "role-rename") {
                embed.Title = "role-rename Help!";
                sb.AppendLine(".role-rename [Role's Current Name] [Role's New Name]");
                sb.AppendLine("");
                sb.AppendLine("if [Role's Current Name] contains quotation marks, then you need to use a backslash to escape.");
                sb.AppendLine(" Ex: I'm \"Serious\" => I'm \\\\\"Serious\\\\\"");
                sb.AppendLine("");
                sb.AppendLine("if [Role's Current Name] is more than one word or contains spaces, it must be surrounded by quotation marks.");
                sb.AppendLine(" Ex: I'm a Bot => \"I'm a Bot\"");
            } else if (cmd == "role-colour" || cmd == "role-color") {
                embed.Title = "role-colour Help!";
                sb.AppendLine(".role-colour [Role's Name] [Hex Colour Code]");
                sb.AppendLine("");
                sb.AppendLine("if [Role's Name] contains quotation marks, then you need to use a backslash to escape.");
                sb.AppendLine(" Ex: I'm \"Serious\" => I'm \\\\\"Serious\\\\\"");
                sb.AppendLine("");
                sb.AppendLine("if [Role's Name] is more than one word or contains spaces, it must be surrounded by quotation marks.");
                sb.AppendLine(" Ex: I'm a Bot => \"I'm a Bot\"");
                sb.AppendLine("");
                sb.AppendLine("[Hex Colour Code] is something like this: #F7A8B8");
            } else {
                embed.Title = "Error";
                sb.AppendLine("Not a vaild command or there is no help info.");
            }
            embed.Description = sb.ToString();
            await ReplyAsync(null, false, embed.Build());
        }

        [Command("role-rename")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task RoleRename(SocketRole role, [Remainder] string newname) {
            var sb = new StringBuilder();
            var oldname = role.Name;
            var rolypoly = role.Guild.GetRole(role.Id);
            await rolypoly.ModifyAsync(x => {
                x.Name = newname;
            });
            sb.AppendLine($"[{oldname}] renamed to [{newname}]!");
            await ReplyAsync(sb.ToString());
        }

        [Command("role-colour")]
        [Alias("role-color")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task RoleColour(SocketRole role, [Remainder] string colour) {
            var sb = new StringBuilder();
            var rolypoly = role.Guild.GetRole(role.Id);
            uint argb = UInt32.Parse(colour.Replace("#", ""), System.Globalization.NumberStyles.HexNumber);
            //colour = colour.Replace("#", "");
            //byte red = (Byte)Convert.ToUInt16(colour.Take(2).ToString(), 16);
            //byte green = (Byte)Convert.ToUInt16(colour.Remove(0, 2).Take(2).ToString(), 16);
            //byte blue = (Byte)Convert.ToUInt16(colour.Remove(0, 4).Take(2).ToString(), 16);
            await rolypoly.ModifyAsync(x => {
                x.Color = new Color(argb);
            });
            sb.AppendLine($"[{role.Name}] coloured to [{colour}]!");
            await ReplyAsync(sb.ToString());
        }

        [Command("hello")]
        public async Task HelloCommand() {
            // initialize empty string builder for reply
            var sb = new StringBuilder();

            // get user info from the Context
            var user = Context.User;

            // build out the reply
            sb.AppendLine($"You are -> [{Context.User.Username}]");
            sb.AppendLine("I must now say, World!");

            // send simple string reply
            await ReplyAsync(sb.ToString());
        }

        [Command("8ball")]
        [Alias("ask")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task AskEightBall([Remainder] string args = null) {
            // I like using StringBuilder to build out the reply
            var sb = new StringBuilder();
            // let's use an embed for this one!
            var embed = new EmbedBuilder();

            // now to create a list of possible replies
            var replies = new List<string>();

            // add our possible replies
            replies.Add("yes");
            replies.Add("no");
            replies.Add("maybe");
            replies.Add("hazzzzy....");

            // time to add some options to the embed (like color and title)
            embed.WithColor(new Color(0, 255, 0));
            embed.Title = "Welcome to the 8-ball!";

            // we can get lots of information from the Context that is passed into the commands
            // here I'm setting up the preface with the user's name and a comma
            sb.AppendLine($",");
            sb.AppendLine();

            // let's make sure the supplied question isn't null 
            if (args == null) {
                // if no question is asked (args are null), reply with the below text
                sb.AppendLine("Sorry, can't answer a question you didn't ask!");
            }
            else {
                // if we have a question, let's give an answer!
                // get a random number to index our list with (arrays start at zero so we subtract 1 from the count)
                var answer = replies[new Random().Next(replies.Count - 1)];

                // build out our reply with the handy StringBuilder
                sb.AppendLine($"You asked: [****]...");
                sb.AppendLine();
                sb.AppendLine($"...your answer is [****]");

                // bonus - let's switch out the reply and change the color based on it
                switch (answer) {
                    case "yes": {
                            embed.WithColor(new Color(0, 255, 0));
                            break;
                        }
                    case "no": {
                            embed.WithColor(new Color(255, 0, 0));
                            break;
                        }
                    case "maybe": {
                            embed.WithColor(new Color(255, 255, 0));
                            break;
                        }
                    case "hazzzzy....": {
                            embed.WithColor(new Color(255, 0, 255));
                            break;
                        }
                }
            }

            // now we can assign the description of the embed to the contents of the StringBuilder we created
            embed.Description = sb.ToString();

            // this will reply with the embed
            await ReplyAsync(null, false, embed.Build());
        }
    }
}